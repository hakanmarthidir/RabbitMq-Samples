using Introduction_Domain;
using Introduction_SharedKernel;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Introduction_Sender
{
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Mevcut degilse olusturur. 
                    channel.QueueDeclare(queue: "introduction-news",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);                    
                    //mesajımız yaratılıyor
                    var model = new News() { NewsId = 1, NewsTitle = "dolar uctu" };
                    //mesaj byte arraye donusur.
                    var message = Toolkit.GetBytes<News>(model);
                    //mesaj ilgili kuyruga gonderilir. 
                    channel.BasicPublish(exchange: "",
                                         routingKey: "introduction-news",
                                         basicProperties: null,
                                         body: message);

                    Console.WriteLine("Sent!");
                }
            }
            Console.ReadLine();
        }
    }
}
