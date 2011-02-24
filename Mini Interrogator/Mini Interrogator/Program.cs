using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;
using DSS.Devices;

namespace Mini_Interrogator
{
    public class Program
    {
        public static void Main()
        {
            ZigBit radio = new ZigBit("COM2");
            // Blink board LED

            bool ledState = false;

            OutputPort led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);

            while (true)
            {

                // Sleep for 500 milliseconds
                Thread.Sleep(500);

                // toggle LED state
                ledState = !ledState;
                led.Write(ledState);
            }
        }

    }
}
