using Introduction_Domain;
using Introduction_SharedKernel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Introduction_Receiver
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
                    //consumer, producerdan once calistirilabilir. bu nedenle bu kuyrugundan varlıgından emin olmak gerekir. 
                    channel.QueueDeclare(queue: "introduction-news",
                                         durable: false, // in memory bilgileri tutar. server restartlarda veriler kaybedilir. true olursa diskte tutar. performance duser
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    //asenkron bir sekilde mesajları almak isteyecegi için callback ihtiyacı ortaya cıkar. receivedin yaptıgı budur. surekli dinler.
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, msg) =>
                    {
                        //byte dizisi olarak alınan mesaj orijinal tipine donusturulur ve bu mesaj uzerinde istenilen işlemler yapılabilir.        
                        var message = Toolkit.GetModel<News>(msg.Body);
                        Console.WriteLine("{0} - {1} ", message.NewsId, message.NewsTitle);

                    };
                    //surekli olarak mesaj gelip gelmedigini dinleyecektir. 
                    channel.BasicConsume(queue: "introduction-news",
                                         autoAck: true, //mesaj alındıktan sonra kuyruktan silinir.
                                         consumer: consumer);

                    Console.ReadLine();
                }
            }
        }
    }
}
