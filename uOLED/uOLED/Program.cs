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

            while (true)
            {
                if (display.AutoBaud())
                {
                    display.SetContrast(0);
                    //display.UpdateDeviceInfo(true);
                   // display.ChangeBackground(new byte[] { 0xFF, 0xFF });
                    display.DrawCircle((byte) 10, (byte)20, (byte)(255), new byte[] { 0xFF, 0xFF });
                    for (byte i = 0; i < 0x0F; i++)
                    {
                        display.SetContrast(i);
                        Thread.Sleep(400);
                    }

                    for (byte i = 0x06; i > 0; i--)
                    {
                        display.SetContrast(i);
                        Thread.Sleep(400);
                    }

                    display.ShutDown(true);
                    Thread.Sleep(500);
                    display.ShutDown(false);
                    Thread.Sleep(300);
                }
            }
        }
    }
}
