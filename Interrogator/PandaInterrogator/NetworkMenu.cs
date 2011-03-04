using System;
using Microsoft.SPOT;
using DSS.Devices;
using System.Threading;

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
        }

        public override void Draw()
        {

            disp.DrawString(0, 0, 2, new byte[] { textColor.msb, textColor.lsb }, "Network Monitor");
        }

        public override void Clear()
        {
            disp.Cls();
        }

        public override void SelectionChanged(byte s)
        {
            selected = s;
            Update();
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
                        disp.DrawString(0, 3, 2, new byte[] { 0xFF, 0xFF }, "Nodes: " + radio.children.Count.ToString());
                        if (radio.children.Count > 0)
                        {
                            if (selected > radio.children.Count - 1) selected = 0;
                            for(int i = 0; i < radio.children.Count; i++)
                            {
                                if(i== selected)
                                    disp.DrawString(0, (byte)(i+5), 2, new byte[] { selectedColor.msb, selectedColor.lsb},"#"+(i+1)+": "+ radio.children[i].ToString() );
                                else disp.DrawString(0, (byte)(i+5), 2, new byte[] { 0xFF, 0xFF },"#"+(i+1)+": "+ radio.children[i].ToString() );
                            }

                            radio.Write((ulong)radio.children[0], "test1234", false);
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
