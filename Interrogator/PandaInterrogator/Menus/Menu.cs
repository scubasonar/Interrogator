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
    public class Menu
    {
        public string title;
        public string[] buttons;
        public Menu[] subMenus;
        public Menu parent;

        public  byte height = 20, width = 60, x = 0, y = 0;
        public uOLED.Color selectedColor = uOLED.BLUE, textColor = uOLED.WHITE;
        public byte selected = 0;
        public byte font = 2;
        public uOLED disp;
        public ZigBit radio;
        public InterruptPort button_up, button_down, button_select, button_back;

        Boolean _active = false; // is this menu active?

        public Boolean active
        {
            get{ return _active;}
            set{
                    if(value && !_active)
                        GotView();
                    else if (!value && _active)
                        LostView();
                    _active = value;
                }
        }

        public Menu()
        {
        }

        public Menu(Menu Parent)
        {
            disp = Parent.disp;
            parent = Parent;
        }

        public Menu(uOLED Disp, ZigBit Radio, InterruptPort Sw1, InterruptPort Sw2)
        {
            disp = Disp;
            radio = Radio;
            button_up = Sw1;
            button_down = Sw2;
        }

       
        public Menu(String[] Buttons, uOLED Disp)
        {
            buttons = Buttons;
            disp = Disp;
        }

        public Menu(String[] Buttons, uOLED Disp, Menu Parent)
        {
            parent = Parent;
            buttons = Buttons;
            disp = Disp;
        }

        public virtual void GotView()
        {
            //selected = 0;
            button_up.OnInterrupt += new NativeEventHandler(button_up_OnInterrupt);
            button_down.OnInterrupt += new NativeEventHandler(button_down_OnInterrupt);
            button_select.OnInterrupt += new NativeEventHandler(button_select_OnInterrupt);
            button_back.OnInterrupt += new NativeEventHandler(button_back_OnInterrupt);

            Draw();
            Thread.Sleep(100);
            button_up.EnableInterrupt();
            button_down.EnableInterrupt();
            button_select.EnableInterrupt();
            button_back.DisableInterrupt();
        }

        

        void button_up_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            SelectionChanged((byte)(selected - 1));
        }

        public virtual void button_down_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            SelectionChanged((byte)(selected + 1));
        }

        public virtual void button_select_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            active = false;
            try
            {
                subMenus[selected].active = true;
            }
            catch
            {
                if (parent != null)
                    parent.active = true;
            }
        }

        void button_back_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            try
            {
                active = false;
                parent.active = true;
            }
            catch
            {
                active = true;
            }
        }

        public virtual void LostView()
        {
            button_up.DisableInterrupt();
            button_down.DisableInterrupt();
            button_select.DisableInterrupt();
            button_back.DisableInterrupt();

            button_up.OnInterrupt -= button_up_OnInterrupt;
            button_down.OnInterrupt -= button_down_OnInterrupt;
            button_select.OnInterrupt -= button_select_OnInterrupt;
            button_back.OnInterrupt -= button_back_OnInterrupt;

            Clear();
        }

        public virtual void Draw()
        {
            disp.Cls();

            if (selected > buttons.Length - 1)
                selected = 0;
            for (byte i = 0; i < buttons.Length; i++)
            {
                if(i== selected)
                    disp.DrawString(0, (byte)(i+5), 2, new byte[] { selectedColor.msb, selectedColor.lsb }, buttons[selected]);
                else
                disp.DrawString(0, (byte)(i+5), 2, new byte[] { textColor.msb, textColor.lsb }, buttons[i]);
            }
        }

        public virtual void Clear() // ideally this would undraw the sections that the menu have drawn so we don't have to clear the whole screen.
        {
            
        }

        // make sure you don't have 255 menu items! 254 is the max. 255 is for going from the top of the list to the bottom. 
        public virtual void SelectionChanged(byte s)
        {
            if ((s == selected) || (buttons == null))
                return;
            else
            {
                if (s == 255) s = (byte)((byte)(buttons.Length) - 1);
                else if (s >= buttons.Length) s = 0;

                disp.DrawString(0, (byte)(selected + 5), 2, new byte[] { textColor.msb, textColor.lsb }, buttons[selected]);
                disp.DrawString(0, (byte)(s + 5), 2, new byte[] { selectedColor.msb, selectedColor.lsb }, buttons[s]);
                selected = s;
            }

        }
    }
}
