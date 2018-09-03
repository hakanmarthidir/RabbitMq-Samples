using RabbitMQ.Client;
using System;

namespace TopicExchange_Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] newsRoutes = { "nyt.news.economy", "bbc.news.history", "nyt.article.sport", "bbc.news.sport", "bbc.news.weather" };
            var rnd = new Random();

            var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Exchange Deklare Edilir. Direct tipinde bir exchange duzenlendi. 
                    channel.ExchangeDeclare(exchange: "newstopic", type: "topic");
                    //Kuyruk Deklare edilir 
                    channel.QueueDeclare(queue: "newstopic_news", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "newstopic_sport", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "newstopic_bbcnewsweather", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //ve Exchange ile İlişkilendirilir.
                    channel.QueueBind(queue: "newstopic_news", exchange: "newstopic", routingKey: "*.news.*");
                    channel.QueueBind(queue: "newstopic_sport", exchange: "newstopic", routingKey: "*.*.sport");
                    channel.QueueBind(queue: "newstopic_bbcnewsweather", exchange: "newstopic", routingKey: "bbc.news.weather");

                    for (int i = 0; i < 20; i++)
                    {
                        var randomNumber = rnd.Next(0, 5);
                        var route = newsRoutes[randomNumber];
                        var message = "News " + i.ToString() + " " + route;
                        var body = System.Text.Encoding.UTF8.GetBytes(message);
                        //Routing key cok onemli
                        Console.WriteLine(route);
                        channel.BasicPublish(exchange: "newstopic", routingKey: route, basicProperties: null, body: body);
                    }
                    Console.WriteLine("All Messages Were Sent!");
                    Console.ReadLine();
                }
            }            
        }
    }
}
