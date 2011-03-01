using System;
using Microsoft.SPOT;

using DSS.Devices;

namespace PandaInterrogator
{
    class Menu
    {
        string[] buttons;
        public Menu[] subMenus;
        public Menu parentMenu;

        byte height = 20, width = 60, x= 0, y=0;
        uOLED.Color selectedColor = uOLED.BLUE, textColor = uOLED.RED;
        public byte selected = 0;
        byte font = 2;
        uOLED disp;

        public Menu(String[] Buttons, uOLED Disp)
        {
            buttons = Buttons;
            disp = Disp;
        }

        public void draw()
        {
            disp.DrawString(0, (byte)(5), 2, new byte[] { selectedColor.msb, selectedColor.lsb }, buttons[0]);
            for (byte i = 1; i < buttons.Length; i++)
                disp.DrawString(0, (byte)(i+5), 2, new byte[] { textColor.msb, textColor.lsb }, buttons[i]);
        }

        public void clear()
        {
            for (byte i = 0; i < buttons.Length; i++)
                disp.DrawString(0, (byte)(i + 5), 2, new byte[] { uOLED.BLACK.msb, uOLED.BLACK.lsb }, buttons[i]);
        }

        public void selectionChanged(byte s)
        {
            if (s == selected)
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
