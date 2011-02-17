using System;
using System.Threading;
using System.Text;
using System.IO.Ports;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.System;

using DSS.Devices;
namespace PandaInterrogator
{
    public class Program
    {
        public static void Main()
        {
            bool ledState = false;
            OutputPort led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);
            uOLED display = new uOLED(new SerialPort("COM2"));
            ZigBit radio = new ZigBit("COM1");
            Random r = new Random();
            

            //SerialPort radio = new SerialPort("COM1", 38400, Parity.None, 8);
            
            //radio.Open();

            // display init
            Thread.Sleep(50);
            
            display.Cls();
            Thread.Sleep(50);
            bool value = display.DrawPolygon(3, new byte[] { 32, 0, 0, 32, 50, 32 }, new byte[] { 0xFF, 0xFF });
            //

            //radio init
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

            while (true)
            {
                int i = 0;
                Thread.Sleep(100);
            }
        }

       

    }
}
