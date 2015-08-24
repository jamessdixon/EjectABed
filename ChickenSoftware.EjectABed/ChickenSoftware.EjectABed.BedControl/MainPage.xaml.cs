
using System;
using Windows.UI.Xaml;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;

namespace ChickenSoftware.EjectABed.BedControl
{
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 5;
        private GpioPin pin;
        private GpioPinValue pinValue;

        private DispatcherTimer lightTimer;
        private Int32 lightTimerCycleCount = 0;
        private DispatcherTimer queueCheckTimer;
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public MainPage()
        {
            InitializeComponent();

            queueCheckTimer = new DispatcherTimer();
            queueCheckTimer.Interval = TimeSpan.FromSeconds(15);
            queueCheckTimer.Tick += QueueCheckTimer_Tick;
            
            if(IsGPIOInitializationSuccessful())
            {
                GpioStatus.Text = "GPIO initialized correctly.  Waiting for next message...";
                queueCheckTimer.Start();
            }
            else
            {
                GpioStatus.Text = "GPIO initializtion failed.";
            }
        }


        private Boolean IsGPIOInitializationSuccessful()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                pin = null;
                return false;
            }
            else
            {
                pin = gpio.OpenPin(LED_PIN);
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                return true;
            }
        }

        private void LightTimer_Tick(object sender, object e)
        {
            switch (lightTimerCycleCount)
            {
                case 0:
                    pinValue = GpioPinValue.Low;
                    pin.Write(pinValue);
                    LED.Fill = redBrush;
                    lightTimerCycleCount += 1;
                    GpioStatus.Text = "Bed Ejected. Resetting...";
                    break;
                default:
                    lightTimer.Stop();
                    lightTimerCycleCount = 0;
                    pinValue = GpioPinValue.High;
                    pin.Write(pinValue);
                    LED.Fill = grayBrush;
                    GpioStatus.Text = "Waiting for next message...";
                    queueCheckTimer.Start();
                    break;
            }
        }

        internal async void QueueCheckTimer_Tick(object sender, object e)
        {
            if (await IsMessageOnQueue())
            {
                queueCheckTimer.Stop();
                GpioStatus.Text = "Message for the EjectABed.  Starting to Eject...";
                RunEjectionSequence();
            }
        }

        internal void RunEjectionSequence()
        {
            lightTimer = new DispatcherTimer();
            lightTimer.Interval = TimeSpan.FromSeconds(20);
            lightTimer.Tick += LightTimer_Tick;

            if (pin != null)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
                LED.Fill = greenBrush;
                lightTimer.Start();
            }
        }

        internal async Task<Boolean> IsMessageOnQueue()
        {
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ejectabed;AccountKey=HZMPnsiL0fzqJunxRswtw5DwQYaa2HRXePkFNg66y0TQanAIkLYGYW5TDoP/CClM1u2UDrp192dlcDoWcxdVbA==";
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference("sloan");
            var queueExists = await queue.ExistsAsync();
            if (!queueExists)
            { 
                GpioStatus.Text = "Queue does not exist or is unreachable.";
                return false;
            }
            var message = await queue.GetMessageAsync(); 
            if (message != null)
            {
                await queue.DeleteMessageAsync(message);
                return true;
            }
            GpioStatus.Text = "No message for the EjectABed.";
            return false;
        }
    }
}
