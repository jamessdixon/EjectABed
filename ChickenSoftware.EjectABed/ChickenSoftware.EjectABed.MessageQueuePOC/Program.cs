using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace ChickenSoftware.EjectABed.MessageQueuePOC
{
    class Program
    {
        static String storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ejectabed;AccountKey=HZMPnsiL0fzqJunxRswtw5DwQYaa2HRXePkFNg66y0TQanAIkLYGYW5TDoP/CClM1u2UDrp192dlcDoWcxdVbA==";

        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Console.WriteLine("Press The 'E' Key To Eject.  Press 'Q' to quit...");

            var keyInfo = ConsoleKey.S;
            do
            {
                keyInfo = Console.ReadKey().Key;
                if (keyInfo == ConsoleKey.E)
                {
                    CreateQueue();
                    WriteToQueue();
                    //ReadFromQueue();
                }

            } while (keyInfo != ConsoleKey.Q);

            Console.WriteLine("End");
            Console.ReadKey();
        }

        private static void CreateQueue()
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference("sloan");
            queue.CreateIfNotExists();
            Console.WriteLine("Created Queue");
        }

        private static void WriteToQueue()
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference("sloan");
            var message = new CloudQueueMessage("Eject!");
            queue.AddMessage(message);
            Console.WriteLine("Wrote To Queue");
        }


        private static void ReadFromQueue()
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference("sloan");
            var queueExists = queue.Exists();
            if (!queueExists)
                Console.WriteLine("Queue does not exist");
            var message = queue.GetMessage();
            if (message != null)
            {
                queue.DeleteMessage(message);
                Console.WriteLine("Message Found and Deleted");
            }
            else
            {
                Console.WriteLine("No messages");
            }
        }
    }
}
