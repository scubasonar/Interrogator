using System;
using Microsoft.SPOT;

namespace PandaInterrogator
{
    class AboutMenu : Menu
    {
        public AboutMenu(Menu Parent) 
        {
            parent = Parent;
            title = "About";
            disp = parent.disp;
            button_up = parent.button_up;
            button_down = parent.button_down;
            button_select = parent.button_select;
            button_back = parent.button_back;
        }

        public override void Draw()
        {
            disp.Cls();
            disp.DrawString(0, 0, 2, new byte[] { textColor.msb, textColor.lsb }, "About");
            disp.DrawString(0, 2, 0, new byte[] { textColor.msb, textColor.lsb }, "Desert Star Systems");
            disp.DrawString(0, 3, 0, new byte[] { textColor.msb, textColor.lsb }, "SeaTag Interrogator");
            disp.DrawString(0, 4, 0, new byte[] { textColor.msb, textColor.lsb }, "for network control");
            disp.DrawString(0, 5, 0, new byte[] { textColor.msb, textColor.lsb }, "and data download.");

            disp.DrawString(0, 9, 0, new byte[] { textColor.msb, textColor.lsb }, "Software by:");
            disp.DrawString(0, 10, 0, new byte[] { textColor.msb, textColor.lsb }, "Matthew Crenshaw");
            disp.DrawString(0, 11, 0, new byte[] { textColor.msb, textColor.lsb }, "mc@desertstar.com");

        }

        public override void Clear()
        {
            disp.Cls();
        }
    }
}
