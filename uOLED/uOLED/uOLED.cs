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
        const  byte GSGC_REPLCOLOR = 0x6B; // replace colour
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


        #region --------------- Graphics -------------------

        public bool SetPenSize(byte size)
        {
            dPort.Write(new byte[] { GSGC_SETPEN, size }, 0, 2);

            return Ack();
        }

        public byte[] ReadPixel(byte x, byte y)
        {
            byte[] color = new byte[2];
            dPort.Write(new byte[] { GSGC_RDPIXEL, x, y },0,3);
            dPort.Read(color, 0, 2);

            return color;
        }

        // xs/ys = top left corner of area to copy from, xd/yd = top left of desitnation, width and height are of area to copy
        public bool ScreenCopyPaste(byte xs, byte ys, byte xd, byte yd, byte width, byte height)
        {
            dPort.Write(new byte[] { GSGC_SCRNCOPYPASTE, xs, ys, xd, yd, width, height }, 0, 7);

            return Ack();
        }

        public bool ReplaceColor(byte x1, byte y1, byte x2, byte y2, byte[] colorOld, byte[] colorNew)
        {
            dPort.Write(new byte[] {GSGC_REPLCOLOR, x1, y1, x2, y2, colorOld[0], colorOld[1], colorNew[0], colorNew[1]}, 0, 9);
            return Ack();
        }

        public bool SetBackground(byte[] color)
        {
            dPort.Write(new byte[] { GSGC_BACKGND, color[0], color[1] }, 0, 3);
            return Ack();
        }

        public bool ReplaceBackground(byte[] color)
        {
            dPort.Write(new byte[] { GSGC_BACKGND, color[0], color[1] }, 0, 3);
            return Ack();
        }

        // saves a 1 byte wide by 8 bytes deep bit map char
        // B7 | B6 | B5 | B4 | B3 | B2 | B1 | B0 | <- data bits
        //    |    |    |    |    |    |    |    | data1
        //    |    |    |    |    |    |    |    | data2
        //    |    |    |    |    |    |    |    | data3
        //    |    |    |    |    |    |    |    | data4
        //    |    |    |    |    |    |    |    | data5
        //    |    |    |    |    |    |    |    | data6
        //    |    |    |    |    |    |    |    | data7
        //    |    |    |    |    |    |    |    | data8
        // char_idx = index between 0 to 31 / 0x00 to 0x1F
        public bool AddUserBitmapChar(byte char_idx, byte[] data)
        {
            if (data.Length > 8)
                return false;
            dPort.Write(new byte[] { GSGC_ADDBM, char_idx }, 0, 2);
            dPort.Write(data, 0, data.Length);
            return Ack();
        }

        public bool Cls()
        {
            dPort.Write(new byte[] { GSGC_CLS }, 0, 1);
            return Ack();
        }

        #region ------Drawing ------------------------------

        public bool DrawPixel(byte x, byte y, byte[] color)
        {
            dPort.Write(new byte[] { GSGC_PIXEL, x, y, color[0], color[1] }, 0, 5);
            return Ack();
        }

        public bool DrawLine(byte x1, byte y1, byte x2, byte y2, byte[] color)
        {
            dPort.Write(new byte[] { GSGC_LINE, x1, y1, x2, y2, color[0], color[1] }, 0, 7);
            return Ack();
        }

        // draws a circle in an anti-clockwise fashion with x1y1 at the top of the triangle. 
        //       1
        //    2
        //           3
        // solid or wireframe drawing determined by 
        // pen size 0 = solid
        // pen size 1 = wireframe
        public bool DrawTriangle(byte x1, byte y1, byte x2, byte y2, byte x3, byte y3, byte[] color)
        {
            dPort.Write(new byte[] { GSGC_TRIANGLE, x1, y1, x2, y2, x3, y3, color[0], color[1] }, 0, 9);
            return Ack();
        }

        // pen size = 0 = solid
        // pen size = 1 = wireframe
        public bool DrawRectangle(byte x1, byte y1, byte x2, byte y2, byte[] color)
        {
            dPort.Write(new byte[] { GSGC_RECTANGLE, x1, y1, x2, y2, color[0], color[1] }, 0, 8);
            return Ack();
        }

        public bool DrawPolygon(byte verticies, byte[] points, byte[] color)
        {
            if ((verticies < 3) || (verticies > 7))
                return false;

            dPort.Write(new byte[] { GSGC_POLYGON, verticies }, 0, 2);
            dPort.Write(points, 0, 2 * verticies);
            dPort.Write(color, 0, 2);

            return Ack();
        }

        public bool DrawCircle(byte x, byte y, byte r, byte[] color)
        {
            dPort.Write(new byte[] { GSGC_CIRCLE, x, y, r, color[0], color[1] }, 0, 6);
            return Ack();
        }

        // 0,0 is at top left of image. use color mode = 0x08 for 256 colors(8bits per pixel), or 0x10 for 65K color mode(16 bits/pixel).
        public bool DrawImage(byte x, byte y, byte width, byte height, byte colorMode, byte[] pixels)
        {
            dPort.Write(new byte[] { GSGC_IMAGE, x, y, width, height, colorMode }, 0, 6);
            dPort.Write(pixels, 0, pixels.Length);

            return Ack();
        }
        #endregion

        #endregion

        #region ------------------ Text -----------------
        public bool SetFont(byte set) // set can be 0x00, 0x01, or 0x02(largest)
        {
            dPort.Write(new byte[] {GSGC_SETFONT, set}, 0, 2);
            return Ack();
        }

        public bool SetOpacity(byte opaq)
        {
            dPort.Write(new byte[] { GSGC_SETOPAQUE, opaq }, 0, 2);
            return Ack();
        }

        public bool DrawChar(byte c, byte column, byte row, byte[] color)
        {
            dPort.Write(new byte[] {GSGC_CHARTXT, c, column, row, color[0], color[1]}, 0, 6);
            return Ack();
        }

        public bool DrawCharGFX(byte c, byte x, byte y, byte[] color, byte width, byte height)
        {
            dPort.Write(new byte[] { GSGC_CHARGFX, x, y, color[0], color[1], width, height }, 0, 7);
            return Ack();
        }

        // row shold be 0-15 for 5x7 and 8x8 fonts, 0-9 for 8x12 fonts
        // column should be 0-20 for 5x7 font, 0-15 for 8x8 and 8x12 fonts
        // font should be 0 for 5x7, 1 for 8x8, and 2 for 8x12
        public bool DrawString(byte column, byte row, byte font, byte[] color, string s)
        {
            byte[] buf = Encoding.UTF8.GetBytes(s);
            dPort.Write(new byte[] { GSGC_STRINGTXT, column, row, font, color[0], color[1] }, 0, 6);
            if (buf.Length > 256)
                dPort.Write(buf, 0, 256);
            else
                dPort.Write(buf, 0, buf.Length);
            dPort.Write(new byte[] { 0x00 }, 0, 1);
            
            return Ack();
        }

        public bool DrawStringGFX(byte x, byte y, byte font, byte[] color, byte width, byte height, string s)
        {
            byte[] buf = Encoding.UTF8.GetBytes(s);
            dPort.Write(new byte[] { GSGC_STRINGGFX, x, y, font, color[0], color[1], width, height }, 0, 6);
            if (buf.Length > 256)
                dPort.Write(buf, 0, 256);
            else
                dPort.Write(buf, 0, buf.Length);
            dPort.Write(new byte[] { 0x00 }, 0, 1);

            return Ack();
        }

        public bool DrawButtonTXT(byte state, byte x, byte y, byte[] btnColor, byte font, byte[] strColor, byte width, byte height, string s)// tested 
        {
            byte[] buf = Encoding.UTF8.GetBytes(s);
            dPort.Write(new byte[] { GSGC_BUTTONTXT, state, x, y, btnColor[0], btnColor[1], font, strColor[0], strColor[1], width, height }, 0, 11);
            dPort.Write(buf, 0, buf.Length);
            dPort.Write(new byte[] { 0x00 }, 0, 1);


            return Ack();
        }

       
        #endregion

        #region -----------------SD/SDHC Memory Card-----------------
        #endregion

        #region ------------------Script Commands(4DSL - Script Language) ------------------
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
        