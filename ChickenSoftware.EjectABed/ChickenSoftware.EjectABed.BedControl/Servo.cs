using System;
using Windows.UI.Xaml;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.System.Threading;
using Windows.Foundation;

namespace ChickenSoftware.EjectABed.BedControl
{
    // Yellow - Signal
    // Black (-) Ground
    // Red (+) 5 Volts
    public class Servo : IBedCommand
    {
        private const int SERVO_PIN = 13;
        private GpioPin pin;
        private GpioPinValue pinValue;
        ThreadPoolTimer timer = null;
        double servoUpPulseWidth = 1250;
        double servoDownPulseWidth = 1750;
        double servoNeutralPulseWidth = 1500;
        double pulseFrequency = 2000;
        double currentPulseWidth = 0;
        Stopwatch stopwatch = null;

        public bool Initialize()
        {
            stopwatch = Stopwatch.StartNew();
            GpioController gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                pin = null;
                return false;
            }
            else
            {
                pin = gpio.OpenPin(SERVO_PIN);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                Windows.System.Threading.ThreadPool.RunAsync(this.MotorThread,
                    Windows.System.Threading.WorkItemPriority.High);
                return true;
            }
        }

        private void MotorThread(IAsyncAction action)
        {
            while(true)
            {
                
            }
        }

        public void Eject()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Sleep()
        {
            throw new NotImplementedException();
        }
    }
}
