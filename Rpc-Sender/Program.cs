using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Rpc_Sender
{

    public class RpcClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;

        public RpcClient()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);
        }

        public string Call(string message)
        {
            var correlationId = Guid.NewGuid().ToString();
            var taskCompletedSource = new TaskCompletionSource<string>();
            var resultTask = taskCompletedSource.Task;          

            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            EventHandler<BasicDeliverEventArgs> handler = null;
            handler = (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    consumer.Received -= handler;

                    var body = ea.Body;
                    var response = Encoding.UTF8.GetString(body);
                    Console.WriteLine(response);
                    taskCompletedSource.SetResult(response);
                }
            };
            consumer.Received += handler;
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: props, body: messageBytes);
            channel.BasicConsume(consumer: consumer, queue: replyQueueName, autoAck: true);
            return resultTask.Result;
        }

        public void Close()
        {
            connection.Close();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var message = "1000";
            var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var replyQueueName = channel.QueueDeclare().QueueName;
                    var consumer = new EventingBasicConsumer(channel);

                    var correlationId = Guid.NewGuid().ToString();
                    var taskCompletedSource = new TaskCompletionSource<string>();
                    var resultTask = taskCompletedSource.Task;

                    var props = channel.CreateBasicProperties();
                    props.CorrelationId = correlationId;
                    props.ReplyTo = replyQueueName;

                    EventHandler<BasicDeliverEventArgs> handler = null;
                    handler = (model, ea) =>
                    {
                        if (ea.BasicProperties.CorrelationId == correlationId)
                        {
                            consumer.Received -= handler;
                            var body = ea.Body;
                            var response = Encoding.UTF8.GetString(body);                           
                            taskCompletedSource.SetResult(response);
                        }
                    };
                    consumer.Received += handler;
                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: props, body: messageBytes);
                    Console.WriteLine("Message was sent!");
                    channel.BasicConsume(consumer: consumer, queue: replyQueueName, autoAck: true);
                    Console.WriteLine("Result : " + resultTask.Result);
                    Console.ReadLine();
                }
            }   
        }
    }
}
