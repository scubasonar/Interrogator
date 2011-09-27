using System;
using Microsoft.SPOT;
using DSS.Devices;
using System.Threading;
using PandaInterrogator.Menus;

namespace PandaInterrogator
{
    class NetworkMenu : Menu
    {
        Boolean connected;
        
        public NetworkMenu(Menu Parent) 
        {
            parent = Parent;
            title = "Network Monitor";
            disp = parent.disp;
            radio = Parent.radio;
            button_up = parent.button_up;
            button_down = parent.button_down;
            button_select = parent.button_select;
            button_back = parent.button_back;
            radio.childrenChanged += new EventHandler(RadioChildrenChanged);
        }

        void RadioChildrenChanged(object sender, EventArgs e)
        {
                subMenus = new NodeMenu[radio.neighbors.Count + 1];
                for (int i = 0; i < subMenus.Length; i++)
                {
                    try
                    {
                        subMenus[i] = new NodeMenu(this, (uint.Parse(radio.neighbors[i].ToString())));
                    }
                    catch
                    {
                    }
                }

            if(active)
                Draw();
        }
        public override void GotView()
        {
            base.GotView();
        }

        public override void LostView()
        {
            base.LostView();
        }

        public override void Draw()
        {
            disp.Cls();
            disp.DrawString(0, 0, 2, new byte[] { textColor.msb, textColor.lsb }, "Network Monitor");
            disp.DrawRectangle((byte)(disp.dInfo.hRes), (byte)disp.dInfo.vRes, (byte)(disp.dInfo.hRes - 10), (byte)(disp.dInfo.vRes - 10), new byte[] { 0x07, 0xE0 });

            disp.DrawString(0, 3, 2, new byte[] { 0xFF, 0xFF }, "Nodes: " + radio.neighbors.Count.ToString());
            if (radio.neighbors.Count > 0)
            {
                if (selected > radio.neighbors.Count) selected = 0;
                for (int i = 0; i < radio.neighbors.Count; i++)
                {
                    if (i == selected)
                        disp.DrawString(0, (byte)(i + 5), 2, new byte[] { selectedColor.msb, selectedColor.lsb }, "#" + (i + 1) + ": " + radio.neighbors[i].ToString());
                    else disp.DrawString(0, (byte)(i + 5), 2, new byte[] { 0xFF, 0xFF }, "#" + (i + 1) + ": " + radio.neighbors[i].ToString());
                }
            }

            if(selected == radio.neighbors.Count)
                disp.DrawString(0, (byte)(radio.neighbors.Count + 5), 2, new byte[] { selectedColor.msb, selectedColor.lsb }, "Back");
            else
                disp.DrawString(0, (byte)(radio.neighbors.Count + 5), 2, new byte[] { textColor.msb, textColor.lsb }, "Back");
        }

        public override void SelectionChanged(byte s)
        {
            if (subMenus == null)
                RadioChildrenChanged(null, null);

            selected = s;
            Draw();
            //Update();
        }


        public void Update()
        {
               
               connected = radio.CheckStatus();
                if (connected)
                {
                        disp.Cls();
                        disp.DrawString(0, 0, 2, new byte[] { textColor.msb, textColor.lsb }, "Network Monitor");
                        disp.DrawRectangle((byte)(disp.dInfo.hRes), (byte)disp.dInfo.vRes, (byte)(disp.dInfo.hRes - 10), (byte)(disp.dInfo.vRes - 10), new byte[] { 0x07, 0xE0 });

                        disp.DrawString(0, 2, 2, new byte[] { 0xFF, 0xFF }, "Status: Online");
                        disp.DrawString(0, 3, 2, new byte[] { 0xFF, 0xFF }, "Nodes: " + radio.neighbors.Count.ToString());
                        if (radio.neighbors.Count > 0)
                        {
                            if (selected > radio.neighbors.Count - 1) selected = 0;
                            for(int i = 0; i < radio.neighbors.Count; i++)
                            {
                                if(i== selected)
                                    disp.DrawString(0, (byte)(i+5), 2, new byte[] { selectedColor.msb, selectedColor.lsb},"#"+(i+1)+": "+ radio.neighbors[i].ToString() );
                                else disp.DrawString(0, (byte)(i+5), 2, new byte[] { 0xFF, 0xFF },"#"+(i+1)+": "+ radio.neighbors[i].ToString() );
                            }

                            radio.Write((ulong)radio.neighbors[0], "test1234", false);
                        }

                        Thread.Sleep(500);
                        connected = radio.CheckStatus();
                }
                else if (active)
                {
                    selected = 0;
                    radio.Join();
                    disp.DrawString(0, 2, 2, new byte[] { 0xFF, 0xFF }, "Status: Offline");
                    disp.DrawRectangle((byte)(disp.dInfo.hRes), (byte)disp.dInfo.vRes, (byte)(disp.dInfo.hRes - 10), (byte)(disp.dInfo.vRes - 10), new byte[] { 0xF8, 0x00 });
                }

                else
                {
                    return;
                }
        }
    }

}
