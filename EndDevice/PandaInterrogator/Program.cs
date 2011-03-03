﻿using System;
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
        const byte btnHeight = 18;
        static byte selectedButton = 0;
        static uOLED display;
        static InputPort sw1;
        static InterruptPort sw2;
        static OutputPort led;
        static bool ledState;
        static bool connected;
        
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
            
            led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);
            //display = new uOLED(new SerialPort("COM1", 9600));
            //display = new uOLED(d);
            //display.ShutDown(false);
            //display.UpdateDeviceInfo(true);

            Thread.Sleep(500);
            ZigBit.Config radioConfig = new ZigBit.Config();
            radioConfig.role = ZigBit.Role.EndDevice;
            radioConfig.addrExtend = 22;
            radioConfig.addrShort = 22;
            radioConfig.baud = 38400;
            radioConfig.commPort = "COM1";
            radioConfig.connected = false;
            radioConfig.dataBits = 8;
            radioConfig.echo = false;
            radioConfig.panID = 1620;
            radioConfig.parity = Parity.None;
            radioConfig.retries = 2;


            ZigBit radio = new ZigBit(radioConfig);
            Random r = new Random();
            radio.dataRX += new EventHandler(radio_dataRX);
            //display.Cls();
            lastAction = DateTime.Now;
            
            while (true)
            {
                connected = radio.CheckStatus();
                if (connected)
                {
                    while (connected)
                    {
                        /*display.Cls();
                        display.DrawRectangle((byte)(display.dInfo.hRes), 0, (byte)(display.dInfo.hRes - 10), 10, new byte[] { 0x07, 0xE0 });
                        
                        display.DrawString(0, 0, 2, new byte[] { 0xFF, 0xFF }, "Nodes: " + radio.children.Count.ToString());
                        if (radio.children.Count > 0)
                        {

                            display.DrawString(0, 2, 0, new byte[] { 0xFF, 0xFF }, "---------------------");
                            display.DrawString(0, 3, 0, new byte[] { 0xFF, 0xFF }, "--Connected devices--");
                            display.DrawString(0, 4, 0, new byte[] { 0xFF, 0xFF }, "---------------------");
                            for(int i = 0; i < radio.children.Count; i++)
                                display.DrawString(0, (byte)(i+5), 0, new byte[] { 0xFF, 0xFF },"#"+(i+1)+": "+ radio.children[i].ToString() );

                            radio.Write((ulong)radio.children[0], "test1234", false);
                        }

                        Thread.Sleep(500);*/
                        connected = radio.CheckStatus();
                    }
                }
                else
                {
                    radio.Join();
                    //display.DrawRectangle((byte)(display.dInfo.hRes), 0, (byte)(display.dInfo.hRes - 10), 10, new byte[] { 0xF8, 0x00 });
                }
            }
        }

        static void radio_dataRX(object sender, EventArgs e)
        {
            ZigBitDataRXEventArgs args = (ZigBitDataRXEventArgs)e;
            //display.DrawString(0, 5, 0, new byte[] { 0xFF, 0xFF }, "RX: " + args.source + "," + args.data);
            Thread.Sleep(1000);
        }

        static void radio_dataRX(object sender, ZigBitDataRXEventArgs e)
        {
            //display.DrawString(0,5,0, new byte[] {0xFF, 0xFF}, "RX: " + e.source + "," + e.data);
            Thread.Sleep(1000);
        }

        static void radio_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort radio = (SerialPort)sender;
            byte[] rx = new byte[radio.BytesToRead];
            string rxs;

            radio.Read(rx, 0, rx.Length);
            rxs = new string(UTF8Encoding.UTF8.GetChars(rx));
            Debug.Print(rxs);
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