using RabbitMQ.Client;
using System;

namespace DirectExchange_Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] newsPortals = { "bbc", "nyt" };
            Random rnd = new Random();

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

                    for (int i = 0; i < 20; i++)
                    {
                        var randomNumber = rnd.Next(0, 2);
                        var route = newsPortals[randomNumber];
                        var message = "News " + i.ToString();
                        var body = System.Text.Encoding.UTF8.GetBytes(message);
                        //Routing key cok onemli
                        Console.WriteLine(route);
                        channel.BasicPublish(exchange: "newsdirect", routingKey: route, basicProperties: null, body: body);
                    }

                    Console.WriteLine("All Messages Were Sent!");
                }
            }
            Console.ReadLine();
        }
    }
}
