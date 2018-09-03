using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace TopicExchange_Receiver1
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
                    channel.ExchangeDeclare(exchange: "newstopic", type: "topic");
                    //Kuyruk Deklare edilir 
                    channel.QueueDeclare(queue: "newstopic_news", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "newstopic_sport", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "newstopic_bbcnewsweather", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //ve Exchange ile İlişkilendirilir.
                    channel.QueueBind(queue: "newstopic_news", exchange: "newstopic", routingKey: "*.news.*");
                    channel.QueueBind(queue: "newstopic_sport", exchange: "newstopic", routingKey: "*.*.sport");
                    channel.QueueBind(queue: "newstopic_bbcnewsweather", exchange: "newstopic", routingKey: "bbc.news.weather");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, msg) =>
                    {
                        var message = System.Text.Encoding.UTF8.GetString(msg.Body);
                        Console.WriteLine(message + " " + msg.RoutingKey);
                    };
                    channel.BasicConsume(queue: "newstopic_news", autoAck: true, consumer: consumer);
                    Console.ReadLine();
                }
            }         
        }
    }
}
