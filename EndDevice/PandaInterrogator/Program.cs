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

namespace EndDevice
{
    public class Program
    {
        static BlinkM light = new BlinkM();
        const byte btnHeight = 18;
        static byte selectedButton = 0;
        static InputPort sw1;
        static InputPort unusedAnalog;
        static InterruptPort sw2;
        static OutputPort led;
        static bool ledState;
        static bool connected;
        static public byte[] currentcolor = { 0, 0, 0 };
        enum colors {RED = 0, BLUE, GREEN};

        static DateTime lastAction;
        public static void Main()
        {
            //SerialPort d = new SerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
            //d.Open();
            // remap COM4 RX (in) pin from P4.29/DIO17 to P0.26 (that is An3)
            // remap COM4 TX (out) pin from P4.28/DIO13 to P0.25 (that is An2)
            Register PINSEL9 = new Register(0xE002C024);
            PINSEL9.Write(0);// COM4 is now disconnected from P4.28 and P4.29
            Register PINSEL1 = new Register(0xE002C004);
            PINSEL1.SetBits(0xf << 18);// COM4 is now connected to An3 and An4

            ledState = false;

            //sw2 = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.Di3, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            //sw1 = new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di2, true, Port.ResistorMode.PullUp);

            //sw1 = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.Di7, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            //sw2 = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.Di8, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            
            
            //sw2.OnInterrupt += sw2_OnInterrupt;
            //sw2.EnableInterrupt();
            Cpu.GlitchFilterTime = new TimeSpan(0, 0, 0, 0, 200);
            //unusedAnalog = new InputPort((Cpu.Pin)FEZ_Pin.AnalogIn.An0, );
            
            led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);
            //display = new uOLED(new SerialPort("COM1", 9600));
            //display = new uOLED(d);
            //display.ShutDown(false);
            //display.UpdateDeviceInfo(true);

            Thread.Sleep(500);
            ZigBit.Config radioConfig = new ZigBit.Config();
            radioConfig.role = ZigBit.Role.EndDevice;
            Random r = new Random();
            int ID = r.Next(1000);
            radioConfig.addrExtend = (ulong)ID;
            radioConfig.addrShort = ID;
            radioConfig.baud = 38400;
            radioConfig.commPort = "COM1";
            connected = false;
            radioConfig.dataBits = 8;
            radioConfig.echo = false;
            radioConfig.panID = 1620;
            radioConfig.parity = Parity.None;
            radioConfig.retries = 2;


            ZigBit radio = new ZigBit(radioConfig);
            radio.dataRX += new EventHandler(radio_dataRX);
            
            //display.Cls();
            lastAction = DateTime.Now;

            light.Blue = 255 ;
            while (true)
            {
                if (radio.connected)
                {
                    for (int i = 0; i < 3; i++)
                    {

                        light.Red = 0;
                        light.Blue = 0;
                        light.Green = 255;
                        Thread.Sleep(100);
                        light.Red = 0;
                        light.Blue = 0;
                        light.Green = 0;
                        Thread.Sleep(500);
                    }


                    while (radio.connected)
                    {
                        Blink();
                        Thread.Sleep(500);
                    }
                }
                else
                {
                    light.Red = 255;
                    light.Blue = 0;
                    light.Green = 0;
                    Thread.Sleep(500);
                    light.Red = 0;
                    light.Blue = 0;
                    light.Green = 0;
                    radio.radio.Close();
                    radio.radio.Open();
                    radio.Init();
                    radio.Join();
                    //display.DrawRectangle((byte)(display.dInfo.hRes), 0, (byte)(display.dInfo.hRes - 10), 10, new byte[] { 0xF8, 0x00 });
                }
            }
        }

        static void radio_dataRX(object sender, EventArgs e)
        {
            ZigBitDataRXEventArgs args = (ZigBitDataRXEventArgs)e;
            //display.DrawString(0, 5, 0, new byte[] { 0xFF, 0xFF }, "RX: " + args.source + "," + args.data);

            try
            {
                string[] data = args.data.Split(new char[] { ',' });
                string color = data[2];

                switch (color)
                {
                    case "RED":
                        currentcolor[0] = 255;
                        currentcolor[1] = 0;
                        currentcolor[2] = 0;
                        break;
                    case "BLUE":
                        currentcolor[0] = 0;
                        currentcolor[1] = 255;
                        currentcolor[2] = 0;
                        break;
                    case "GREEN":
                        currentcolor[0] = 0;
                        currentcolor[1] = 0;
                        currentcolor[2] = 255;
                        break;
                    case "OFF":
                        currentcolor[0] = 0;
                        currentcolor[1] = 0;
                        currentcolor[2] = 0;
                        break;
                }
            }
            catch
            { return; }


            Thread.Sleep(1000);
        }

        static void SetColor()
        {
            light.Red = currentcolor[0];
            light.Blue = currentcolor[1];
            light.Green = currentcolor[2];
        }


        static void Blink()
        {
            light.Red = currentcolor[0];
            light.Blue = currentcolor[1];
            light.Green = currentcolor[2];
            Thread.Sleep(100);
            light.Red = 0;
            light.Blue = 0;
            light.Green = 0;
            Thread.Sleep(300);
        }

    }
}
