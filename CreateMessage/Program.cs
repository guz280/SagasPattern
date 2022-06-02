using Contract;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace CreateMessage
{
	internal class Program
	{
		static void Main(string[] args)
		{
            var exit = false;


            do
            {
                Console.WriteLine("**************************************");
                Console.WriteLine("*               MENU                 *");
                Console.WriteLine("**************************************");
                Console.WriteLine("*  1. Create message                 *");
                Console.WriteLine("*  X. Exit                           *");
                Console.WriteLine("**************************************");

                switch (Console.ReadKey().KeyChar.ToString().ToUpper())
                {
                    case "1":
                        var docId = Guid.NewGuid();
                        Console.WriteLine($"CreateNewFile: {docId}");
                        


                        Send(new UploadDocumentContext()
                        {
                            DocumentPublicId = docId,
                            TransactionId = Guid.NewGuid(),
                            CustomerId = 1,
                            FilePath = null,
                            FileLocation = FileLocation.AtJumio,
                            FileUploadStatus = FileUploadSteps.NotStarted
                        });
                        break;
                    case "X":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Do not know what you are writing");
                        break;
                }

                System.Threading.Thread.Sleep(1000);

                Console.Clear();
            } while (!exit);
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
