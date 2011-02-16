using System;
using System.Text;
using System.IO.Ports;
using Microsoft.SPOT;

namespace DSS.Devices
{
    // ZigBit Serial Net
    class ZigBit
    {
#region ----------------------------------------- COMMANDS  ------------------------------------------------------------------------- 
        //----------------Networking Prameters--------------------
        const string CMD_PANID = "+WPANID"; // Extended PAN ID
        const string CMD_CHANNEL = "+WPANID"; // Active channel
        const string CMD_CHMASK = "+WCHMASK"; // Channel mask
        const string CMD_CHPAGE = "+WCHPAGE"; // Channel page
        const string CMD_AUTONET = "+WAUTONET"; // Automatic networking
        const string CMD_ROLE = "+WROLE"; // Node role
        const string CMD_ADDRE = "+GSN"; // Device extended address
        const string CMD_ADDR = "+WSRC"; // Node short address
        
        //---------------Network Management-----------------------
        const string CMD_JOIN = "+WJOIN"; // Start/Join to network
        const string CMD_LEAVE = "+WLEAVE"; // Leave the network
        const string CMD_STATUS = "+WNWK"; //Request networking status
        const string CMD_PARENT = "+WPARENT"; // Request parent address
        const string CMD_CHILD = "+WCHILDREN"; // Request children addresses
        const string CMD_SIZE = "+WNBSIZE"; // Request number of neighbor nodes
        const string CMD_NEIGHBOR = "+WNB"; // Request neighbor info
        const string CMD_ADDRMOD = "S30"; // Network addressing mode(special because it doesn't need a +)
        const string CMD_LQI = "+WLQI"; // Request LQI
        const string CMD_RSSI = "+WRSSI"; // Request RSSI

        //---------------Power Management-----------------------
        const string CMD_PWR = "+WPWR"; // End device sleep paramaters
        const string CMD_SLEEP = "+WSLEEP"; // Force sleep
        const string CMD_TXPWR = "+WTXPWR"; // TX power level

        //---------------Data Transmission-----------------------
        const string CMD_TX = "D"; // Transmit data to a specific node
        const string CMD_TXB = "DU"; // Broadcast transmit
        const string CMD_TXS = "DS"; // Send S-register value to specific node
        const string CMD_PING = "+WPING"; // Ping a node
        const string CMD_SYNCPRD = "+WSYNCPRD"; // Indirect poll rate
        const string CMD_TIMEOUT = "+WTIMEOUT"; // Data delivery time-out
        const string CMD_RETRY = "+WRETRY"; // Repetition count
        const string CMD_WAIT = "+WWAIT"; // Data transmission waiting time-out;

        //---------------Generic Control-----------------------
        const string CMD_RESET = "Z"; // Warm reset
        const string CMD_HELP = "&H"; // Help
        const string CMD_PARAMS = "%H"; // Display paramaters and S-register values;
        const string CMD_IDPROD = "I"; // Display product identification info
        const string CMD_IDMANU = "+GMI"; // Request for manufacturer identitifaction info
        const string CMD_IDMODEL = "+GMM"; // Request for model identification
        const string CMD_IDHWSW = "+GMR"; // Request for hardware/software revision identification
        const string CMD_DEFAULTS = "&F"; // Set to factory-defined configuration

        //---------------Host interface commands-----------------------
        const string CMD_CHART = "S3"; // Termination character 
        const string CMD_CHARR = "S4"; // Response formatting character
        const string CMD_CHARE = "S5"; // Command editing character
        const string CMD_ECHO = "E"; // Command echo
        const string CMD_SUPPRESS = "Q"; // Result code suppression
        const string CMD_FORMAT = "V"; // Response format
        const string CMD_CODE = "X"; // Result code selection
        const string CMD_BAUD = "+IPR"; // Serial port communication rate
        const string CMD_FLOW = "+IFC"; // Serial port flow control
        const string CMD_DTR = "&D"; // DTR Behavior
        const string CMD_LAST = "S0"; // Request the latest result code

        /*---------------Hardware Control-----------------------
        // GPIO configuration
        // GPIO
        // A/D configuration
        // A/D
        // PWM Configuration
        // PWM frequency control
        // PWM duty cycle control
        */

        //---------------Remote management-----------------------
        const string CMD_PWD = "+WPASSWORD"; // Set a password
        const string CMD_REMOTE = "R"; // Remote execution of AT command


#endregion

        SerialPort radio;

        public ZigBit(string comPort)
        {
            
            radio = new SerialPort(comPort, 38400, Parity.None, 8);

        }

        public bool Ack()
        {

            return false;

        }

    }
}
