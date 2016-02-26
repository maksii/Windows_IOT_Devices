using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace L298N
{
    public class L298N
    {
        public L298N(int pinFor = 18, int pinBack = 23)
        {
            MotorOneForward = pinFor;
            MotorOneBackward = pinBack;
        }

        public int MotorOneForward;
        public int MotorOneBackward;
        public GpioPin StepOnePinForward;
        public GpioPin StepOnePinBackward;
        private GpioOpenStatus _status;
        public string StatusMsg;

        public void Main()
        {
            InitGPIO();

            // Call MotorMove(GpioPin pin, int time) and pass your motor pin and time to wheel it.
        }

        private void InitGPIO()
        {
            GpioController gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                StepOnePinForward = null;
                StepOnePinBackward = null;

                StatusMsg = "There is no GPIO controller on this device.";
                Debug.WriteLine(StatusMsg);
                return;
            }

            CheckAndOpenPin(MotorOneForward, gpio);
            CheckAndOpenPin(MotorOneBackward, gpio);
            StatusMsg = "GPIO pin initialized correctly.";
            Debug.WriteLine(StatusMsg);
        }

        private void CheckAndOpenPin(int pin, GpioController gpio)
        {
            gpio.TryOpenPin(MotorOneForward, GpioSharingMode.Exclusive, out StepOnePinForward, out _status);
            if (_status == GpioOpenStatus.PinOpened)
            {
                StepOnePinForward = gpio.OpenPin(MotorOneForward);
                StepOnePinForward.Write(GpioPinValue.Low);
                StepOnePinForward.SetDriveMode(GpioPinDriveMode.Output);
            }
            else
            {
                StatusMsg = "Pin Unavailable or getting Sharing Violation";
                Debug.WriteLine(StatusMsg);
            }
        }

        async Task PutTaskDelay(int time)
        {
            await Task.Delay(time);
        }

        private void ChangeValue(GpioPinValue value, GpioPin pin)
        {
            pin.Write(value);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public async void MotorMove(GpioPin pin, int time)
        {
            ChangeValue(GpioPinValue.High, pin);
            await PutTaskDelay(time);
            ChangeValue(GpioPinValue.Low, pin);
        }
    }
}
