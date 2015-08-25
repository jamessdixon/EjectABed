using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Windows;

namespace ChickenSoftware.EjectABed.UserControl.WPF
{
    public partial class MainWindow : Window
    {
        static String storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ejectabed;AccountKey=HZMPnsiL0fzqJunxRswtw5DwQYaa2HRXePkFNg66y0TQanAIkLYGYW5TDoP/CClM1u2UDrp192dlcDoWcxdVbA==";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ejectButton_Click(object sender, RoutedEventArgs e)
        {
            CreateQueue();
            WriteToQueue();

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


    }
}
