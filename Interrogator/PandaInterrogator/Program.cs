using System;
using System.Threading;
using System.Text;
using System.IO.Ports;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.System;
using GHIElectronics.NETMF.Hardware.LowLevel;

using DSS.Devices;

namespace PandaInterrogator
{
    public class Program
    {
        const byte btnHeight = 18;
        static byte selectedButton = 0;
        static uOLED display;
        static InterruptPort sw1;
        static InterruptPort sw2;
        static OutputPort led;
        static bool ledState;
        static bool connected;
        static ZigBit radio;

        static DateTime lastAction;
        static Menu mainMenu;
        static Menu aboutMenu; 
        static Menu currentMenu;

        public static void Main()
        {
            SerialPort d = new SerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
            d.Open();
            // remap COM4 RX (in) pin from P4.29/DIO17 to P0.26 (that is An3)
            // remap COM4 TX (out) pin from P4.28/DIO13 to P0.25 (that is An2)
            Register PINSEL9 = new Register(0xE002C024);
            PINSEL9.Write(0);// COM4 is now disconnected from P4.28 and P4.29
            Register PINSEL1 = new Register(0xE002C004);
            PINSEL1.SetBits(0xf << 18);// COM4 is now connected to An3 and An4

            ledState = false;

            //sw2 = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.Di3, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            //sw1 = new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di2, true, Port.ResistorMode.PullUp);

            sw1 = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.Di7, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            sw2 = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.Di8, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            
            sw1.OnInterrupt += sw1_OnInterrupt;
            sw1.EnableInterrupt();
            sw2.OnInterrupt += sw2_OnInterrupt;
            sw2.EnableInterrupt();
            Cpu.GlitchFilterTime = new TimeSpan(0, 0, 0, 0, 100);
            
            led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);
            //display = new uOLED(new SerialPort("COM1", 9600));
            display = new uOLED(d);
            display.ShutDown(false);
            display.UpdateDeviceInfo(true);

            Thread.Sleep(500);
            radio = new ZigBit("COM2");
            Random r = new Random();
            radio.dataRX += new EventHandler(radio_dataRX);
            display.Cls();
            lastAction = DateTime.Now;

            
            mainMenu = new MainMenu(display, radio, sw1, sw2);
            currentMenu = mainMenu;
            mainMenu.active = true;
            while (true)
            {
                Thread.Sleep(200);
            }
        }

        static void radio_dataRX(object sender, EventArgs e)
        {
            ZigBitDataRXEventArgs args = (ZigBitDataRXEventArgs)e;
            display.DrawString(0, 5, 0, new byte[] { 0xFF, 0xFF }, "RX: " + args.source + "," + args.data);
            Thread.Sleep(1000);
        }

        static void sw2_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            lastAction = DateTime.Now;
        }

        static void sw1_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            lastAction = DateTime.Now;
        }

        static void screenSaver()
        {
            Random r = new Random();
            display.Cls();
            display.DrawString(0, 0, 0, new byte[] { 0xFF, 0xFF }, "Going to sleep");
            Thread.Sleep(250);
            for (byte i = 0; i < 4; i++)
            {
                display.DrawString((byte)(14 + i), 0, 0, new byte[] { 0xFF, 0xFF }, ".");
                Thread.Sleep(250);
            }

            display.Cls();
            display.ShutDown(true);

            led.Write(ledState = false);
            Thread.Sleep(100);
            Power.Hibernate(Power.WakeUpInterrupt.InterruptInputs);
            //while(DateTime.Now > lastAction.AddSeconds(1));
            Thread.Sleep(100);
            bool success = display.ShutDown(false);
            Thread.Sleep(500);
            display.DrawString(0, 0, 0, new byte[] { 0xFF, 0xFF }, "Waking up.");
            for (byte i = 0; i < 4; i++)
            {
                display.DrawString((byte)(10 + i), 0, 0, new byte[] { 0xFF, 0xFF }, ".");
                Thread.Sleep(250);
            }

            display.Cls();
            currentMenu.Draw();
        }

    }
}
