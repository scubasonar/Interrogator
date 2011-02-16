using System;
using System.Threading;
using System.Text;
using System.IO.Ports;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using uOLED;

using GHIElectronics.NETMF.FEZ;

namespace PandaInterrogator
{
    public class Program
    {
        public static void Main()
        {
            uOLED.uOLED display;
            SerialPort disp = new SerialPort("COM2", 9600);
            disp.Open();
            SerialPort radio = new SerialPort("COM1", 38400, Parity.None, 8);
            
            Random r = new Random();
            bool ledState = false;

            OutputPort led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);

            
            disp.ReadTimeout = 100;
            radio.Open();

            // display init
            Thread.Sleep(50);
            display = new uOLED.uOLED(disp);
            display.Cls();
            Thread.Sleep(50);
            bool value = display.DrawPolygon(3, new byte[] { 32, 0, 0, 32, 50, 32 }, new byte[] { 0xFF, 0xFF });
            //

            //radio init
            radio.Write(Encoding.UTF8.GetBytes("ATX\n"), 0, 4);
            Thread.Sleep(5);
            radio.Write(Encoding.UTF8.GetBytes("AT+GSN=1\n"), 0, 9);
            Thread.Sleep(5);
            radio.Write(Encoding.UTF8.GetBytes("AT+WPANID=1620\n"), 0, 15);
            Thread.Sleep(5);
            radio.Write(Encoding.UTF8.GetBytes("AT+WROLE=0 +WSRC=0\n"), 0, 19);
            Thread.Sleep(5);
            radio.Write(Encoding.UTF8.GetBytes("AT+WJOIN\n"), 0, 9);
            Thread.Sleep(5);
            while (true)
            {
                bool x;
                x = display.DrawButtonTXT(0x01, 0, 0, new byte[] { 0x0F, 0xBF }, 0x00, new byte[] { 0xBB, 0x00 }, 1, 1, "hello");
                //display.DrawChar((byte)'F', 0, 0, new byte[] { 0xFF, 0xFF });
                //display.DrawStringGFX(0, 0, 0, new byte[] { 0xFF, 0xFF }, 5, 7, "hello");
                Thread.Sleep(300);
                x = display.DrawButtonTXT(0x00, 0, 0, new byte[] { 0x0F, 0xBF }, 0x00, new byte[] { 0xCF, 0xDF }, 1, 1, "hello");
                Thread.Sleep(300);
            }
        }

       

    }
}
