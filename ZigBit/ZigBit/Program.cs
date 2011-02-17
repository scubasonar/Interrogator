using System;
using System.Threading;
using System.Text;
using System.IO.Ports;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;
using DSS.Devices;

namespace ZigBitTest
{
    public class Program
    {
        static ZigBit radio;

        public static void Main()
        {
            radio = new ZigBit("COM1");
            bool ledState = false;
            byte[] rx;
            string rxStr;
            int count;

            OutputPort led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);

            

           
            while (true)
            {
                count = radio.radio.BytesToRead;
                rx = new byte[count];
                if(count > 0)
                {
                    radio.radio.Read(rx, 0, count);
                    rxStr = new string(Encoding.UTF8.GetChars(rx));
                    Debug.Print(rxStr);
                }
                
                // Sleep for 500 milliseconds
                Thread.Sleep(500);

                // toggle LED state
                ledState = !ledState;
                led.Write(ledState);
            }
        }

    }
}
