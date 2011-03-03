using System;
using Microsoft.SPOT;
using DSS.Devices;

namespace PandaInterrogator
{
    class NetworkMenu : Menu
    {
        public NetworkMenu(Menu Parent) 
        {
            parent = Parent;
            title = "Network Monitor";
            disp = parent.disp;
        }

        public override void Draw()
        {
            disp.DrawString(0, 0, 2, new byte[] { textColor.msb, textColor.lsb }, "Network Monitor");
        }

        public override void Clear()
        {
            disp.DrawString(0, 0, 2, new byte[] { 0x00, 0x00 }, "Network Monitor");
        }

    }
}
