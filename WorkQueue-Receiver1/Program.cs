using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace WorkQueue_Receiver1
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "workqueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);                    
                    consumer.Received += (model, msg) =>
                    {
                        var message = System.Text.Encoding.UTF8.GetString(msg.Body);
                        Console.WriteLine(message);                        
                        channel.BasicAck(deliveryTag: msg.DeliveryTag, multiple: false);
                    };
                    
                    channel.BasicConsume(queue: "workqueue", autoAck: false, consumer: consumer);                    
                    Console.ReadLine();
                }
            }
        }
    }
}
