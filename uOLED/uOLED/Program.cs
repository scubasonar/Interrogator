using System;
using System.Threading;
using System.Text;
using System.IO.Ports;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;

namespace uOLED
{
    public class Program
    {
        public static void Main()
        {
            uOLED display;
            SerialPort disp = new SerialPort("COM1", 9600);
            disp.ReadTimeout = 100;
            Random r = new Random();
            bool ledState = false;

            OutputPort led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);
            
            disp.Open();
            Thread.Sleep(50);
            display = new uOLED(disp);
            display.Cls();
            Thread.Sleep(50);
            bool value = display.DrawPolygon(3, new byte[] { 32, 0, 0, 32, 50, 32}, new byte[] { 0xFF, 0xFF });
           
            
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
