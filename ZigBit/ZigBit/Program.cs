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
        static ZigBit r;

        public static void Main()
        {
            r = new ZigBit("COM2");
            r.Ack();
            SerialPort radio = new SerialPort("COM1", 38400, Parity.None, 8);
            bool ledState = false;
            byte[] rx;
            string rxStr;
            int count;

            radio.Open();
            OutputPort led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);
            
            //radio init
            radio.Write(Encoding.UTF8.GetBytes("ATE\r"), 0, 4);
            radio.Write(Encoding.UTF8.GetBytes("AT+WLEAVE\r"), 0, 10);
            Thread.Sleep(200);
            radio.Write(Encoding.UTF8.GetBytes("ATX\r\n"), 0, 5);
            Thread.Sleep(200);
            radio.Write(Encoding.UTF8.GetBytes("AT+GSN=1\r\n"), 0, 10);
            Thread.Sleep(200);
            radio.Write(Encoding.UTF8.GetBytes("AT+WPANID=1620\r"), 0, 15);
            Thread.Sleep(200);
            radio.Write(Encoding.UTF8.GetBytes("AT+WCHMASK=100000\r"), 0, 18);
            Thread.Sleep(200);
            radio.Write(Encoding.UTF8.GetBytes("AT+WROLE=0 +WSRC=0\r"), 0, 19);
            Thread.Sleep(200);
            radio.Write(Encoding.UTF8.GetBytes("AT+WJOIN\r"), 0, 9);
            Thread.Sleep(200);
            // Blink board LED

           
            while (true)
            {
                count = radio.BytesToRead;
                rx = new byte[count];
                if(count > 0)
                {
                    radio.Read(rx, 0, count);
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
