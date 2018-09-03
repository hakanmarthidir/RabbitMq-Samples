using RabbitMQ.Client;
using System;

namespace WorkQueue_Sender
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
                    channel.QueueDeclare(queue: "workqueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    for (int i = 0; i < 1000; i++)
                    {
                        var message = "Message " + i.ToString();
                        var body = System.Text.Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "", routingKey: "workqueue", basicProperties: properties, body: body);
                    }
                    
                    Console.WriteLine("All Messages Were Sent!");
                }
            }            
            Console.ReadLine();
        }
    }
}
