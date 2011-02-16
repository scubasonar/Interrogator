using System;
using System.Text;
using System.IO.Ports;

using Microsoft.SPOT;

namespace DSS.Devices
{
    class ZigBit
    {
        SerialPort radio;

        public ZigBit(string com)
        {
            radio = new SerialPort(com, 38400, Parity.None, 8);

        }

    }
}
