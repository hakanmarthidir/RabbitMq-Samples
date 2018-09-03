using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace HeaderExchange_Receiver1
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

                    //X-Match in anlamı tum optionları barındırması zorunludur. 
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

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, msg) =>
                    {
                        var message = System.Text.Encoding.UTF8.GetString(msg.Body);
                        Console.WriteLine(message);
                    };
                    //BBC kuyrugundan gerekli haberleri alıp istedigini yapabilir. Aynı mesajı farklı işlemler için kullanabiliriz. 
                    channel.BasicConsume(queue: "newsheader_sport_news", autoAck: true, consumer: consumer);
                    Console.ReadLine();
                }
            }
            
        }
    }
}
