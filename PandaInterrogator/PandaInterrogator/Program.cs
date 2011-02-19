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
        static InputPort sw1;
        static InterruptPort sw2;
        static OutputPort led;
        static bool ledState;
        static DateTime lastAction;
        public static void Main()
        {
            
            ledState = false;
            sw2 = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.Di3, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            sw1 = new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di2, true, Port.ResistorMode.PullUp);
            //sw2 = new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di3, true, Port.ResistorMode.PullUp);
            //InterruptPort sw1 = new InterruptPort((Cpu.Pin)FEZ_Pin.Digital.Di2, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLevelLow);
            //sw1.OnInterrupt += new NativeEventHandler(sw1_OnInterrupt);
            //sw3.EnableInterrupt();
            sw2.OnInterrupt += sw2_OnInterrupt;
            Cpu.GlitchFilterTime = new TimeSpan(0, 0, 0, 0, 200);
            sw2.EnableInterrupt();

            led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);
            display = new uOLED(new SerialPort("COM2", 115200));
            display.ShutDown(false);

            Thread.Sleep(500);
            ZigBit radio = new ZigBit("COM1");
            Random r = new Random();
            
            display.Cls();
            drawButtons();
            lastAction = DateTime.Now;
            while (true)
            {
                if(DateTime.Now > lastAction.AddSeconds(5))
                    screenSaver();
                Thread.Sleep(500);
            }
        }

        static void screenSaver()
        {
            Random r = new Random();
            display.Cls();
            display.DrawString(0, 0, 0, new byte[] { 0xFF, 0xFF }, "Going to sleep");
            Thread.Sleep(250);
            for (byte i = 0; i < 4; i++)
            {
                display.DrawString((byte)(14+i), 0, 0, new byte[] { 0xFF, 0xFF }, ".");
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
            //Thread.Sleep(500);
            display.DrawString(0, 0, 0, new byte[] { 0xFF, 0xFF }, "Waking up.");
            for (byte i = 0; i < 4; i++)
            {
                display.DrawString((byte)(10 + i), 0, 0, new byte[] { 0xFF, 0xFF }, ".");
                Thread.Sleep(250);
            }

            drawButtons();
        }

        static void sw2_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            
            byte last = selectedButton;
            selectedButton++;
            if (selectedButton > 8) selectedButton = 0;
            updateButtons(last, selectedButton);

            led.Write(ledState = !ledState);

            lastAction = DateTime.Now;
            return;
        }

        public static void drawButtons()
        {
            selectedButton = 0;
            display.Cls();
            display.DrawButtonTXT(0x00, 0, 0, new byte[] { 0xFF, 0xFF }, 0x00, new byte[] { 0xBB, 0x00 }, 1, 1, "Button # 0" );
            for (int i = 1; i < 9; i++)
            {
                display.DrawButtonTXT(0x01, 0, (byte)(btnHeight * i), new byte[] { 0x0F, 0xBF }, 0x00, new byte[] { 0xAA, 0x00 }, 1, 1, "Button # " + i);
            }

        }

        public static void updateButtons(byte last, byte current)
        {
            if (current == last) return;
            display.DrawButtonTXT(0x01, 0, (byte)(btnHeight * last), new byte[] { 0x0F, 0xBF }, 0x00, new byte[] { 0xAA, 0x00 }, 1, 1, "Button # " + last);
            display.DrawButtonTXT(0x00, 0, (byte)(btnHeight * current), new byte[] { 0xFF, 0xFF }, 0x00, new byte[] { 0xBB, 0x00 }, 1, 1, "Button # " + current);
            /*for (byte i = 0; i < 9; i++)
            {
                if(i == selectedButton)
                    display.DrawButtonTXT(0x01, 0, (byte)(btnHeight * i), new byte[] { 0xFF, 0xFF }, 0x00, new byte[] { 0xBB, 0x00 }, 1, 1, "Button # " + i);
                else
                    display.DrawButtonTXT(0x00, 0, (byte)(btnHeight * i), new byte[] { 0x0F, 0xBF }, 0x00, new byte[] { 0xAA, 0x00 }, 1, 1, "Button # " + i);

                
            }*/
         
        }

        static void sw1_OnInterrupt(Cpu.Pin port, bool state, TimeSpan time)
        {
        }

        static void sw1_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            
            throw new NotImplementedException();
        }

       

    }
}
