using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace HeaderExchange_Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            

            var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Exchange Deklare Edilir. Direct tipinde bir exchange duzenlendi. 
                    channel.ExchangeDeclare(exchange: "newsheader", type: "headers");
                    //Kuyruk Deklare edilir 
                    channel.QueueDeclare(queue: "newsheader_sport_news", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "newsheader_all_articles", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //X-Match in anlamı tum optionları barındırması zorunludur. all yerine any de yazılabilirdi. 
                    Dictionary<string, object> newsHeader = new Dictionary<string, object>();
                    newsHeader.Add("x-match", "all");
                    newsHeader.Add("content", "news");
                    newsHeader.Add("category", "sport");

                    Dictionary<string, object> articleHeader = new Dictionary<string, object>();
                    articleHeader.Add("x-match", "all");
                    articleHeader.Add("content", "article");
                    articleHeader.Add("category", "all");

                    //ve Exchange ile İlişkilendirilir.
                    channel.QueueBind(queue: "newsheader_sport_news", exchange: "newsheader", routingKey: "", arguments: newsHeader);
                    channel.QueueBind(queue: "newsheader_all_articles", exchange: "newsheader", routingKey: "", arguments: articleHeader);

                    //BasicOptions Headerlarına atama yapılır. 
                    var newsBasicOptions = channel.CreateBasicProperties();
                    newsBasicOptions.Headers = newsHeader;

                    var articleBasicOptions = channel.CreateBasicProperties();
                    articleBasicOptions.Headers = articleHeader;

                    //News icin Publish 
                    for (int i = 0; i < 20; i++)
                    {                    
                        var message = "News " + i.ToString();
                        var body = System.Text.Encoding.UTF8.GetBytes(message);                        
                        channel.BasicPublish(exchange: "newsheader", routingKey: "", basicProperties: newsBasicOptions, body: body);
                    }

                    //Article icin Publish 
                    for (int i = 0; i < 20; i++)
                    {
                        var message = "Articles " + i.ToString();
                        var body = System.Text.Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "newsheader", routingKey: "", basicProperties: articleBasicOptions, body: body);
                    }

                    Console.WriteLine("All Messages Were Sent!");
                    Console.ReadLine();
                }
            }            
        }
    }
}
