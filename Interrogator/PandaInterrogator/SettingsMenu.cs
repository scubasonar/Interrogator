using System;
using Microsoft.SPOT;

namespace PandaInterrogator
{
    class SettingsMenu : Menu
    {
         public SettingsMenu(Menu Parent) 
        {
            parent = Parent;
            title = "Settings";
            disp = parent.disp;
            radio = Parent.radio;
            sw1 = parent.sw1;
            sw2 = parent.sw2;

        }

        public override void Draw()
        {
            disp.Cls();
            disp.DrawString(0, 0, 2, new byte[] { textColor.msb, textColor.lsb }, title);
            disp.DrawString(0, 2, 2, new byte[] { textColor.msb, textColor.lsb }, "Radio");
            disp.DrawString(0, 3, 2, new byte[] { textColor.msb, textColor.lsb }, "PAN: " + radio.config.panID);
            switch (radio.config.role)
            {
                case DSS.Devices.ZigBit.Role.Coordinator:
                    disp.DrawString(0, 4, 2, new byte[] { textColor.msb, textColor.lsb }, "Role: Co-ord");
                    break;
                case DSS.Devices.ZigBit.Role.Router:
                    disp.DrawString(0, 4, 2, new byte[] { textColor.msb, textColor.lsb }, "Role: Router");
                    break;
                case DSS.Devices.ZigBit.Role.EndDevice:
                    disp.DrawString(0, 4, 2, new byte[] { textColor.msb, textColor.lsb }, "Role: End");
                    break;
            }
            
            disp.DrawString(0, 5, 2, new byte[] { textColor.msb, textColor.lsb }, "ID: " + radio.config.addrShort);
           
        }

        public override void Clear()
        {
            disp.Cls();
            //disp.DrawString(0, 0, 2, new byte[] { 0x00, 0x00 }, title);
        }
    }
}
