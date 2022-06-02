using Contract;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace SagasPattern
{
    internal class Program
    {
        public static FileUploadSteps? FailAt = null;

        static void Main(string[] args)
        {

            ConnectionFactory connectionFactory = new ConnectionFactory();


            IConnection connection = connectionFactory.CreateConnection();


            IModel channel = connection.CreateModel();

            channel.QueueDeclare(RabbitMQData.Queue, false, false, false, null);
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                if (HandleJobStep(JsonSerializer.Deserialize<UploadDocumentContext>(message)))
                    channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(queue: RabbitMQData.Queue,
                                 autoAck: false,
                                 consumer: consumer);

            Console.ReadKey();

        }



        public static bool HandleJobStep(UploadDocumentContext context)
        {
            if(FailAt!=null && FailAt.Value==context.FileUploadStatus)
                return false;

            switch (context.FileUploadStatus)
            {
                case FileUploadSteps.NotStarted:
                    //Do Stuff
                    Thread.Sleep(1000);                    
                    context.FileUploadStatus = FileUploadSteps.AcquireFiles;
                    Send(context);
                    break;
                case FileUploadSteps.AcquireFiles:
                    Thread.Sleep(1000);
                    context.FileUploadStatus = FileUploadSteps.CreateSQLRelation;
                    Send(context);
                    break;
                case FileUploadSteps.CreateSQLRelation:
                    Thread.Sleep(1000);
                    context.FileUploadStatus = FileUploadSteps.UploadDocumentsInStore;
                    Send(context);
                    break;
                case FileUploadSteps.UploadDocumentsInStore:
                    Thread.Sleep(1000);
                    context.FileUploadStatus = FileUploadSteps.CreateTheSupportingStructureInMongoDb;
                    Send(context);                    
                    break;
                case FileUploadSteps.CreateTheSupportingStructureInMongoDb:
                    Thread.Sleep(1000);
                    break;
                default:
                    break;
            }

            return true;
        }

        public static void Send(UploadDocumentContext data)
        {
            var payload = JsonSerializer.Serialize(data);

            ConnectionFactory connectionFactory = new ConnectionFactory();

            using (IConnection connection = connectionFactory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(RabbitMQData.Queue, false, false, false, null);
                    channel.BasicPublish(RabbitMQData.Exchange, "*", null, Encoding.UTF8.GetBytes(payload));
                }
            }

        }
    }
}
