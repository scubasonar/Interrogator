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
        }

        public override void Draw()
        {
            disp.DrawString(0, 0, 2, new byte[] { textColor.msb, textColor.lsb }, "About");
            disp.DrawString(0, 2, 0, new byte[] { textColor.msb, textColor.lsb }, "Desert Star Systems");
            disp.DrawString(0, 3, 0, new byte[] { textColor.msb, textColor.lsb }, "SeaTag Interrogator");
            disp.DrawString(0, 4, 0, new byte[] { textColor.msb, textColor.lsb }, "For downloading tag");
            disp.DrawString(0, 5, 0, new byte[] { textColor.msb, textColor.lsb }, "data.");

            disp.DrawString(0, 9, 0, new byte[] { textColor.msb, textColor.lsb }, "Software by:");
            disp.DrawString(0, 10, 0, new byte[] { textColor.msb, textColor.lsb }, "Matthew Crenshaw");

        }

        public override void Clear()
        {
            disp.Cls();
        }
    }
}
