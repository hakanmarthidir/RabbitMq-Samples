using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace DirectExchange_Receiver2
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
                    channel.ExchangeDeclare(exchange: "newsdirect", type: "direct");
                    //Kuyruk Deklare edilir 
                    channel.QueueDeclare(queue: "newsdirect_bbc", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "newsdirect_nyt", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //ve Exchange ile İlişkilendirilir.
                    channel.QueueBind(queue: "newsdirect_bbc", exchange: "newsdirect", routingKey: "bbc");
                    channel.QueueBind(queue: "newsdirect_nyt", exchange: "newsdirect", routingKey: "nyt");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, msg) =>
                    {
                        var message = System.Text.Encoding.UTF8.GetString(msg.Body);                        
                        Console.WriteLine(message);
                    };                   
                    channel.BasicConsume(queue: "newsdirect_nyt", autoAck: true, consumer: consumer);
                    Console.ReadLine();
                }
            }
           
        }
    }
}
