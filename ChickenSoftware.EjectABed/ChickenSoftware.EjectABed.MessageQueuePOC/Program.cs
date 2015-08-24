using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace ChickenSoftware.EjectABed.MessageQueuePOC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            CreateQueue();
            WriteToQueue();
            //ReadFromQueue();
            Console.WriteLine("End");
            Console.ReadKey();
        }

        private static void CreateQueue()
        {
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ejectabed;AccountKey=sxALfPT8vnTH5iAhVMe2ki0AG5+zWUqKEYpzMeYlaCOEMlq1AL2wqNzus1VbBf599RF3nnylhX7tdYlellK49g==";
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference("sloan");
            queue.CreateIfNotExists();
            Console.WriteLine("Created Queue");
        }

        private static void WriteToQueue()
        {
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ejectabed;AccountKey=sxALfPT8vnTH5iAhVMe2ki0AG5+zWUqKEYpzMeYlaCOEMlq1AL2wqNzus1VbBf599RF3nnylhX7tdYlellK49g==";
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference("sloan");
            var message = new CloudQueueMessage("Eject!");
            queue.AddMessage(message);
            Console.WriteLine("Wrote To Queue");
        }


        private static void ReadFromQueue()
        {
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ejectabed;AccountKey=sxALfPT8vnTH5iAhVMe2ki0AG5+zWUqKEYpzMeYlaCOEMlq1AL2wqNzus1VbBf599RF3nnylhX7tdYlellK49g==";
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
