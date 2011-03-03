using System;
using Microsoft.SPOT;

using DSS.Devices;

namespace PandaInterrogator
{
    class Menu
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

        public Menu()
        {
        }

        public Menu(Menu Parent)
        {
            disp = Parent.disp;
            parent = Parent;
        }

        public Menu(uOLED Disp, ZigBit Radio)
        {
            disp = Disp;
            radio = Radio;
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

        public virtual void Draw()
        {
            disp.DrawString(0, (byte)(5), 2, new byte[] { selectedColor.msb, selectedColor.lsb }, buttons[0]);
            for (byte i = 1; i < buttons.Length; i++)
                disp.DrawString(0, (byte)(i+5), 2, new byte[] { textColor.msb, textColor.lsb }, buttons[i]);
        }

        public virtual void Clear()
        {
            for (byte i = 0; i < buttons.Length; i++)
                disp.DrawString(0, (byte)(i + 5), 2, new byte[] { uOLED.BLACK.msb, uOLED.BLACK.lsb }, buttons[i]);
        }

        public void SelectionChanged(byte s)
        {
            if ((s == selected) || (buttons == null))
                return;
            else
            {
                if (s < 0) s = (byte)buttons.Length;
                else if (s >= buttons.Length) s = 0;

                disp.DrawString(0, (byte)(selected + 5), 2, new byte[] { textColor.msb, textColor.lsb }, buttons[selected]);
                disp.DrawString(0, (byte)(s + 5), 2, new byte[] { selectedColor.msb, selectedColor.lsb }, buttons[s]);
                selected = s;

            }

        }
    }
}
