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
    class MainMenu : Menu
    {
        DispatcherTimer clock;
        public MainMenu(uOLED Disp, ZigBit Radio, InterruptPort Button_up, InterruptPort Button_down, InterruptPort Button_select, InterruptPort Button_back)
        {
            button_up = Button_up;
            button_down = Button_down;
            button_select = Button_select;
            button_back = Button_back;

            radio = Radio;
            disp = Disp;
            buttons = new String[] { "Network", "About", "Settings" };
            subMenus = new Menu[] { new NetworkMenu(this), new AboutMenu(this), new SettingsMenu(this) };
        }

        public override void Draw()
        {
            base.Draw();
        }

    }
}
