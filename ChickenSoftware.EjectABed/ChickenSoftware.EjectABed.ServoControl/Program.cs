using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace ChickenSoftware.EjectABed.ServoControl
{
    public class Program
    {

        private static PWM _servo = null;
        private static InputPort _trigger = null;
        private static bool _servoReady = false;
        private const uint SERVO_UP = 1250;
        private const uint SERVO_DOWN = 1750;
        private const uint SERVO_NEUTRAL = 1500;
        

        public static void Main()
        {
            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
            SetUpTrigger();
            SetUpServo();

            while (true)
            {
                led.Write(true);
                Thread.Sleep(250);
                led.Write(false);
                Thread.Sleep(250);
            }
        }

        internal static void SetUpTrigger()
        {
            _trigger = new InputPort(Pins.GPIO_PIN_D10, false, Port.ResistorMode.PullDown);
            _trigger.OnInterrupt += _trigger_OnInterrupt;

        }

        private static void _trigger_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            _servoReady = false;
            ActivateServoForBellows("UP", 20000);
            ActivateServoForBellows("DOWN", 20000);
            _servoReady = true;
        }

        internal static void SetUpServo()
        {
            uint period = 20000;
            uint duration = SERVO_NEUTRAL;
            _servo = new PWM(PWMChannels.PWM_PIN_D5, period, duration, PWM.ScaleFactor.Microseconds, false);
            _servo.Start();
            _servoReady = true;
        }

        internal static void ActivateServoForBellows(String direction, Int32 duration)
        {
            if(_servoReady)
            {
                if (direction == "UP")
                {
                    _servo.Duration = SERVO_UP;

                }
                else if (direction == "DOWN")
                {
                    _servo.Duration = SERVO_DOWN;

                }
                else
                {
                    _servo.Duration = SERVO_NEUTRAL;
                }
                Thread.Sleep(duration);
                _servo.Duration = SERVO_NEUTRAL;

            }
        }
    }
}
