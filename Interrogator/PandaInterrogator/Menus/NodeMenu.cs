using System;
using Microsoft.SPOT;
using DSS.Devices;

namespace PandaInterrogator.Menus
{
    public class NodeMenu : Menu
    {
        Boolean connected;
        public uint id;

        public NodeMenu(Menu Parent, uint ID)
        {
            parent = Parent;
            title = "Node " + ID;
            disp = parent.disp;
            radio = Parent.radio;
            id = ID;
            buttons = new string[] { "RED", "BLUE", "GREEN", "OFF", "Back"};
            button_up = parent.button_up;
            button_down = parent.button_down;
            button_select = parent.button_select;
            button_back = parent.button_back;
        }

        public override void button_select_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (buttons[selected] == "Back")
            {
                active = false;
                parent.active = true;
                return;
            }
            else
            {
                radio.Write((ulong)id, "SET,COLOR," + buttons[selected], false);
            }

        }

        public override void GotView()
        {
            base.GotView();
            radio.dataRX += new EventHandler(radio_dataRX);
        }

        public override void LostView()
        {
            base.LostView();
            radio.dataRX -= radio_dataRX;
        }

        void radio_dataRX(object sender, EventArgs e)
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
                        disp.DrawString(7, 9, 2, new byte[] { uOLED.RED.msb, uOLED.RED.lsb }, "Received");
                        disp.DrawRectangle(60, 50, 110, 100, new byte[] { uOLED.RED.msb, uOLED.RED.lsb });
                        break;
                    case "BLUE":
                        disp.DrawString(7, 9, 2, new byte[] { uOLED.BLUE.msb, uOLED.BLUE.lsb }, "Received");
                        disp.DrawRectangle(60, 50, 110, 100, new byte[] { uOLED.BLUE.msb, uOLED.BLUE.lsb });
                        break;
                    case "GREEN":
                        disp.DrawString(7, 9, 2, new byte[] { uOLED.GREEN.msb, uOLED.GREEN.lsb }, "Received");
                        disp.DrawRectangle(60, 50, 110, 100, new byte[] { uOLED.GREEN.msb, uOLED.GREEN.lsb });
                        break;
                    case "OFF":
                        disp.DrawString(7, 9, 2, new byte[] { uOLED.WHITE.msb, uOLED.WHITE.lsb }, "Received");
                        disp.DrawRectangle(60, 50, 110, 100, new byte[] { uOLED.BLACK.msb, uOLED.BLACK.lsb });
                        break;

                }
            }
            catch
            { return; }
        }
        /*public void Draw()
        {
            disp.Cls();
            disp.DrawString(0, 0, 2, new byte[] { textColor.msb, textColor.lsb }, title);
            disp.DrawRectangle((byte)(disp.dInfo.hRes), (byte)disp.dInfo.vRes, (byte)(disp.dInfo.hRes - 10), (byte)(disp.dInfo.vRes - 10), new byte[] { 0x07, 0xE0 });
        }*/

    }
}
