using System;
using System.Text;

using Microsoft.SPOT;
using System.IO.Ports;


namespace uOLED
{
    public struct DeviceInfo
    {
        public byte deviceType;
        public byte hwRev;
        public byte firmwareRev;
        public int hRes; // horizontal resolution
        public int vRes; // vertical resolution
    }

    public class uOLED
    {
        #region Command codes

        #region GSGC PROTOCOL CONSTANTS
        const  byte ACK = 0x06; // Acknowledge
        const  byte NAK = 0x15; // Not Acknowledge
        #endregion

        #region GSGC SWITCH-JOYSTICK CONSTANTS
        const  byte SW1_UP =0x10; // SW1 or Joystick UP
        const  byte SW2_LEFT = 0x20; // SW2 or Joystick LEFT
        const  byte SW3_DOWN = 0x30; // SW3 or Joystick DOWN
        const  byte SW4_RIGHT = 0x40; // SW4 or Joystick RIGHT
        const  byte SW5_FIRE = 0x50; // SW5 or Joystick FIRE
        #endregion

        #region GSGC GRAPHICS CONSTANTS
        const  byte COLOR8 = 0x08; // 8 bit Colour Mode
        const  byte COLOR16 = 0x10; // 16 bit Colour Mode
        const  byte BUTTONUP = 0x01; // Button Up Mode
        const  byte BUTTONDOWN = 0x00; // Button Down Mode
        const  int RED = 0xF800; // RED
        const  int GREEN = 0x07E0; // GREEN
        const  int BLUE = 0x001F; // BLUE
        const  int BLACK = 0x0000; // BLACK
        const int WHITE = 0xFFFF; // WHITE
        #endregion


        #region GSGC TEXT CONSTANTS
        const  byte FONT1 = 0x00; // 5x7 Internal Font
        const  byte FONT2 = 0x01; // 8x8 Internal Font
        const  byte FONT3 = 0x02; // 8x12 Internal Font
        #endregion


        #region GSGC GENERAL COMMANDS DEFINITIONS
        const  byte GSGC_AUTOBAUD = 0x55; // Auto Baud Command
        const  byte GSGC_VERSION = 0x56; // Device Info Request
        const  byte GSGC_BACKGND = 0x42; // Change Background Colour
        const  byte GSGC_CLS = 0x45; // Clear Screen
        const  byte GSGC_DISPCONT = 0x59; // Display Control Functions
        const  byte GSGC_SWITCHSTAT = 0x4A; // Get Switch-Buttons Status
        const  byte GSGC_SWITCHSTATWAIT = 0x6A; // Get Switch-Buttons Status with Timeout
        const  byte GSGC_SOUND = 0x4E; // Generate a Tone
        #endregion

        #region GSGC GRAPHICS COMMANDS DEFINITIONS
        const  byte GSGC_ADDBM = 0x41; // Add User Bitmap
        const  byte GSGC_CIRCLE = 0x43; // Draw Circle
        const  byte GSGC_BM = 0x44; // Draw User Bitmap
        const  byte GSGC_TRIANGLE = 0x47; // Draw Triangle
        const  byte GSGC_IMAGE = 0x49; // Draw Image-Icon
        const  byte GSGC_LINE = 0x4C; // Draw Line
        const  byte GSGC_PIXEL = 0x50; // Draw Pixel
        const  byte GSGC_RDPIXEL = 0x52; // Read Pixel
        const  byte GSGC_SCRNCOPYPASTE = 0x63; // Screen Copy-Paste
        const  byte GSGC_POLYGON = 0x67; // Draw Polygon
        const  byte GSGC_SETPEN = 0x70; // Set Pen Size
        const  byte GSGC_RECTANGLE = 0x72; // Draw Rectangle
        #endregion

        #region GSGC TEXT COMMANDS DEFINITIONS
        const  byte GSGC_SETFONT = 0x46; // Set Font
        const  byte GSGC_SETOPAQUE = 0x4F; // Set Transparent-Opaque Text
        const  byte GSGC_STRINGGFX = 0x53; // “String” of ASCII Text (graphics format)
        const  byte GSGC_CHARTXT = 0x54; // ASCII Character (text format)
        const  byte GSGC_BUTTONTXT = 0x62; // Text Button
        const  byte GSGC_STRINGTXT = 0x73; // “String” of ASCII Text (text format)
        const  byte GSGC_CHARGFX = 0x74; // ASCII Character (graphics format)
        #endregion

        // GSGC EXTENDED COMMANDS HEADER DEFINITION
        const  byte GSGC_EXTCMD = 0x40; // Extended Command Header

        #region GSGC MEMORY CARD COMMANDS DEFINITIONS
        const byte GSGC_MCAP  = 0x41; // Set Address Pointer of Memory Card
        const byte GSGC_MCCOPYSAVE = 0x43; // Screen Copy-Save to Memory Card
        const byte GSGC_MCIMAGE = 0x49; // Display Image-Icon from Memory Card
        const byte GSGC_MCOBJ = 0x4F; // Display Object from Memory Card
        const byte GSGC_MCRUN = 0x50; // Run Script (4DSL) Program from Card
        const byte GSGC_MCRDSECTOR = 0x52; // Read Sector Block Data from Memory Card
        const byte GSGC_MCVIDEO = 0x56; // Display Video Clip from Memory Card
        const byte GSGC_MCWRSECTOR = 0x57; // Write Sector Block Data to Memory Card
        const byte GSGC_MCINIT = 0x69; // Initialise Memory Card
        const byte GSGC_MCRDBYTE = 0x72; // Read Byte Data from Memory Card
        const byte GSGC_MCWRBYTE = 0x77; // Write Byte Data to Memory Card
        #endregion

        #region GSGC SCRIPTING COMMANDS DEFINITIONS
        //const byte GSGC_MCAP = 0x41; // Set Address Pointer of Memory Card
        const byte GSGC_DELAY = 0x07; // Delay
        const byte GSGC_SETCNTR = 0x08; // Set Counter
        const byte GSGC_DECCNTR = 0x09; // Decrement Counter
        const byte GSGC_JMPNZ = 0x0A; // Jump to Address If Counter Not Zero
        const byte GSGC_JMP = 0x0B; // Jump to Address
        const byte GSGC_EXIT = 0x0C; // Exit-Terminate Script Program
        #endregion

        #endregion

        public DeviceInfo dInfo;
        SerialPort dPort;

        bool commError = false;

        public uOLED(SerialPort p)
        {
            dPort = p;
            AutoBaud();
        }

        

        #region ---------------- General ------------------------
        /*
         * const byte GSGC_AUTOBAUD = 0x55; // Auto Baud Command
        const byte GSGC_VERSION = 0x56; // Device Info Request
        const byte GSGC_BACKGND = 0x42; // Change Background Colour
        const byte GSGC_CLS = 0x45; // Clear Screen
        const byte GSGC_DISPCONT = 0x59; // Display Control Functions
        const byte GSGC_SWITCHSTAT = 0x4A; // Get Switch-Buttons Status
        const byte GSGC_SWITCHSTATWAIT = 0x6A; // Get Switch-Buttons Status with Timeout
        const byte GSGC_SOUND = 0x4E; // Generate a Tone
        */
        public bool AutoBaud()
        {
            dPort.Write(new byte[] {GSGC_AUTOBAUD}, 0, 1);
            return Ack();
        }


        /// <summary>
        /// Updates the device info data struct showing screen res etc
        /// </summary>
        /// <param name="show"></param>
        /// <returns>true if the device info updates, false otherwise</returns>
        public bool UpdateDeviceInfo(bool show)
        {
            byte[] ret = new byte[5]{0,0,0,0,0};
            DeviceInfo result = new DeviceInfo();

            dPort.Flush();
            if (show)
                dPort.Write(new byte[] { GSGC_VERSION, 0x01 }, 0, 2);
            else
                dPort.Write(new byte[] { GSGC_VERSION , 0x00 }, 0, 2);



            if (Ack() && (dPort.Read(ret, 0, 5) == 5))
            {
                result.deviceType = ret[0];
                result.hwRev = ret[1];
                result.firmwareRev = ret[2];
                #region set horizontal resolution
                switch (ret[3])
                {
                    case 0x22:
                        result.hRes = 220;
                        break;
                    case 0x28:
                        result.hRes = 128;
                        break;
                    case 0x32:
                        result.hRes = 320;
                        break;
                    case 0x60:
                        result.hRes = 160;
                        break;
                    case 0x64:
                        result.hRes = 64;
                        break;
                    case 0x76:
                        result.hRes = 176;
                        break;
                    case 0x96:
                        result.hRes = 96;
                        break;
                    default:
                        return false;
                        break;
                }
                #endregion


                #region set vertical resolution
                switch (ret[4])
                {
                    case 0x22:
                        result.vRes = 220;
                        break;
                    case 0x28:
                        result.vRes = 128;
                        break;
                    case 0x32:
                        result.vRes = 320;
                        break;
                    case 0x60:
                        result.vRes = 160;
                        break;
                    case 0x64:
                        result.vRes = 64;
                        break;
                    case 0x76:
                        result.vRes = 176;
                        break;
                    case 0x96:
                        result.vRes = 96;
                        break;
                    default:
                        return false;
                        break;
                }
                #endregion

                dInfo = result;
                return true;
            }

            return false;
        }

        public bool ChangeBackground(byte[] color)
        {
            dPort.Write(new byte[] { GSGC_BACKGND, color[0], color[1] }, 0, 3);
            return Ack();
        }

        // clear screen
        public bool Cls()
        {
            dPort.Write(new byte[] { GSGC_CLS }, 0, 1);
            return Ack();
        }

        public bool On(bool on)
        {
            if (on)
                dPort.Write(new byte[] {GSGC_DISPCONT, 0x01, 0x01},0,3);

            else
                dPort.Write(new byte[] { GSGC_DISPCONT, 0x01, 0x00 }, 0, 3);
            return Ack();
        }

        // sets contrast (range: 0x00 - 0x0F)
        public bool SetContrast(byte val)
        {
            dPort.Write(new byte[] { GSGC_DISPCONT, 0x02, val }, 0, 3);
            return Ack();
        }

        // switch to low power mode and turn off the display  (10.25mA for .96" uOLED off, 30mA some green text, 115.67mA full white screen max brightness)
        public bool ShutDown(bool off)
        {
            if (!off)
                dPort.Write(new byte[] { GSGC_DISPCONT, 0x03, 0x01 }, 0, 3);

            else
                dPort.Write(new byte[] { GSGC_DISPCONT, 0x03, 0x00 }, 0, 3);
            return (Ack());
        }


        #endregion


        #region --------------- Drawing -------------------
        public bool DrawCircle(byte x, byte y, byte r, byte[] color)
        {
            dPort.Write(new byte[] { GSGC_CIRCLE, x, y, r, color[0], color[1] }, 0, 6);
            return Ack();
        }


        #endregion

        #region ------------------ Text -----------------

        #endregion

        #region --------------------- Communication  ---------------------

        /// <summary>
        /// Receives ACK/NACK from uOLED 
        /// </summary>
        /// <returns>True if it receives ACK, False for NACK or read timeout</returns>
        bool Ack()
        {
            byte[] ret = new byte[] { 0x15 };
            dPort.Read(ret, 0, 1);
            dPort.Flush();
            switch (ret[0])
            {
                case ACK:
                    return true;
                    break;
                case NAK:
                    return false;
                    break;
            }
            return false;
        }

        void Write(string s)
        {
            byte[] buf = Encoding.UTF8.GetBytes(s);
            int offset = 0;
            int ret = 0;
            int len = buf.Length;
            while (len > 0)
            {
                ret = dPort.Write(buf, offset, len);
                len -= ret;
                offset += ret;
            }

        }
        #endregion
    }
}
        