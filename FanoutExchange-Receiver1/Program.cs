using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace FanoutExchange_Receiver1
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
                    //Exchange Deklare Edilir. Fanout tipinde bir exchange duzenlendi. 
                    channel.ExchangeDeclare(exchange: "newsfanout", type: "fanout");
                    //Kuyruk Deklare edilir 
                    channel.QueueDeclare(queue: "newsfanout_bbc", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "newsfanout_newyorktimes", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //ve Exchange ile İlişkilendirilir.
                    channel.QueueBind(queue: "newsfanout_bbc", exchange: "newsfanout", routingKey: "");
                    channel.QueueBind(queue: "newsfanout_newyorktimes", exchange: "newsfanout", routingKey: "");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, msg) =>
                    {
                        var message = System.Text.Encoding.UTF8.GetString(msg.Body);
                        Console.WriteLine(message);                        
                    };
                    //BBC kuyrugundan gerekli haberleri alıp istedigini yapabilir. Aynı mesajı farklı işlemler için kullanabiliriz. 
                    channel.BasicConsume(queue: "newsfanout_bbc", autoAck: true, consumer: consumer);
                    Console.ReadLine();
                }
            }
        }
    }
}
