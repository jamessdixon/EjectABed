using System;
using Windows.UI.Xaml;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;

namespace ChickenSoftware.EjectABed.BedControl
{
    public class SingleLight : IBedCommand
    {
        private const int LED_PIN = 5;
        private GpioPin pin;
        private GpioPinValue pinValue;

        public Boolean Initialize()
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
                pin.SetDriveMode(GpioPinDriveMode.Output);
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
                return true;
            }
        }


        public void Eject()
        {
            if (pin != null)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
            }
        }

        public void Reset()
        {
            if (pin != null)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
            }

        }

        public void Sleep()
        {
            if (pin != null)
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
            }
        }
    }
}
