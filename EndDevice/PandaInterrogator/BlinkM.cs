using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;

namespace GardenLight
{
    class BlinkM
    {
        byte _red = 0, _blue = 0, _green = 0;

        //byte[] init = new byte[4] { (byte)'c', 0xff, 0xc4, 0x30 };

        I2CDevice i2c;

        I2CDevice.I2CTransaction[] xActions;


        public BlinkM()
        {
            i2c = new I2CDevice(new I2CDevice.Configuration(0x00, 100));
        }

        public byte Red
        {
            get
            {return _red;}
            set
            {
                xActions = new I2CDevice.I2CTransaction[4]
                {
                    I2CDevice.CreateWriteTransaction(new byte[] {(byte)'c'}),
                    I2CDevice.CreateWriteTransaction(new byte[] { _red = value }), // red
                    I2CDevice.CreateWriteTransaction(new byte[] { _green }), // blue
                    I2CDevice.CreateWriteTransaction(new byte[] { _blue }) // green
                };

                i2c.Execute(xActions, 100);
            }

        }

        public byte Blue
        {
            get
            { return _blue; }
            set
            {
                xActions = new I2CDevice.I2CTransaction[4]
                {
                    I2CDevice.CreateWriteTransaction(new byte[] {(byte)'c'}),
                    I2CDevice.CreateWriteTransaction(new byte[] { _red }), // red
                    I2CDevice.CreateWriteTransaction(new byte[] { _green }), // green
                    I2CDevice.CreateWriteTransaction(new byte[] { _blue = value}) // blue
                };

                i2c.Execute(xActions, 100);
            }

        }

        public byte Green
        {
            get
            { return _green; }
            set
            {
                xActions = new I2CDevice.I2CTransaction[4]
                {
                    I2CDevice.CreateWriteTransaction(new byte[] {(byte)'c'}),
                    I2CDevice.CreateWriteTransaction(new byte[] { _red }), // red
                    I2CDevice.CreateWriteTransaction(new byte[] { _green = value }), // green
                    I2CDevice.CreateWriteTransaction(new byte[] { _blue }) // blue
                };

                i2c.Execute(xActions, 100);
            }

        }
    }
}
