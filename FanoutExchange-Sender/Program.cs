using RabbitMQ.Client;
using System;

namespace FanoutExchange_Sender
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
                    //Exchange Deklare Edilir. Fanout tipinde bir exchange duzenlendi. 
                    channel.ExchangeDeclare(exchange: "newsfanout", type: "fanout");
                    //Kuyruk Deklare edilir 
                    channel.QueueDeclare(queue: "newsfanout_bbc", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queue: "newsfanout_newyorktimes", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //ve Exchange ile İlişkilendirilir.
                    channel.QueueBind(queue: "newsfanout_bbc", exchange: "newsfanout", routingKey: "");
                    channel.QueueBind(queue: "newsfanout_newyorktimes", exchange: "newsfanout", routingKey: "");

                    for (int i = 0; i < 100; i++)
                    {
                        var message = "News " + i.ToString();
                        var body = System.Text.Encoding.UTF8.GetBytes(message);
                        //Routing key burada onemsizlesiyor. Exchange dagitim işini devraldıgı için artık onun sorumlulugunda. 
                        channel.BasicPublish(exchange: "newsfanout", routingKey: "", basicProperties: null, body: body);
                    } 

                    Console.WriteLine("All Messages Were Sent!");
                    Console.ReadLine();
                }
            }
            
        }
    }
}
