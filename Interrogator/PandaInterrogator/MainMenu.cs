using System;
using Microsoft.SPOT;

using DSS.Devices;

namespace PandaInterrogator
{
    class MainMenu : Menu
    {
        public MainMenu(uOLED Disp, ZigBit Radio)
        {
            radio = Radio;
            disp = Disp;
            buttons = new String[] { "Network", "About", "Settings" };
            subMenus = new Menu[] { new NetworkMenu(this), new AboutMenu(this), new SettingsMenu(this) };
        }

    }
}
