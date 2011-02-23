using System;
using System.Threading;
using System.Text;
using System.IO.Ports;
using Microsoft.SPOT;
using System.Collections;

namespace DSS.Devices
{
    
    // ZigBit Serial Net
    public class ZigBit
    {
        public ArrayList children;

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

        const int INITRETRIES = 3;

        byte[] rx = new byte[256];
        byte rxcnt = 0;

        public string LastCmd = "";
        bool _GotResponse = false;
        public bool GotResponse
        {
            get
            {
                if (_GotResponse)
                {
                    _GotResponse = false;
                    return true;
                }
                return false;
            }

            set
            {
                _GotResponse = value;
            }
        }

        bool _GotACK = false;
        // ack flag that gets cleared on read
        public bool GotACK
        {
            get
            {
                if (_GotACK)
                {
                    _GotACK = false;
                    return true;
                }
                return false;
            }

            set
            {
                _GotACK = value;
            }
        }


        public enum Role
        {
            Coordinator, // 1 per network, manages everything
            Router, // lots per network, should be on all the time to route end device data
            EndDevice // intermittent opeartion
        }

        public struct Config
        {
            // SERIAL PORT
            public string commPort;
            public int baud;
            public Parity parity;
            public short dataBits;
            
            public bool connected;

            // RADIO SETTINGS
            public bool echo;
            public ulong addrExtend; // extended address
            public int addrShort; // short address
            public Role role; // cole(Co-ordinator, router, end device)
            public ulong panID; // ID for the network (all devices must have matching panID's to talk)
            public int retries; // number of tries to do things like ping
        }

        public SerialPort radio;
        public Config config;

        public ZigBit(string portName)
        {
            children = new ArrayList();
            config = new Config();
            config.commPort = portName;
            config.baud = 38400;
            config.parity = Parity.None;
            config.dataBits = 8;
            config.connected = false;
            config.echo = false;
            config.addrExtend = 1;
            config.addrShort = 1;
            config.role = Role.Coordinator;
            config.panID = 1620;
            config.retries = 2;
           
            if (Open())
            {
                if (!Init())
                {
                    Leave();
                    Init();
                }

            }
        }

        public ZigBit(Config c)
        {
            config = c;
            
            if (Open())
            {
                if (!Init())
                {
                    Leave();
                    Init();
                }
            }
        }

        public bool Open()
        {
            radio = new SerialPort(config.commPort, config.baud, config.parity, config.dataBits);
            radio.Open();
            if (radio.IsOpen)
            {
                radio.DataReceived += new SerialDataReceivedEventHandler(radio_DataReceived);
            }
            return radio.IsOpen;
        }

        void processCommand()
        {
            int test;
            string cmd = new string(UTF8Encoding.UTF8.GetChars(rx));
            
            string[] args;
            try
            {
                cmd = cmd.Substring(0, rxcnt);
            }
            catch
            {
                return;
            }

            Debug.Print(cmd);
            
            args = cmd.Split(new char[] { ':', ' ' });
            switch (args[0])
            {
                case "ERROR":
                    GotResponse = true;
                    LastCmd = args[0];
                    GotACK = false;
                    break;
                case "OK":
                    GotResponse = true;
                    LastCmd = args[0];
                    GotACK = true;
                    break;
                case "EVENT":
                    processEvent(args);
                    break;
                case "DATA":
                    processData(args);
                    break;
                case "+WCHILDREN":
                    processChildren(args);
                    break;
                default:
                    break;
            }
            rxcnt = 0;
            return;
        }

        void processChildren(string[] args)
        {
            if (args[1] == "")
            {
                children.Clear();
                return;
            }

            string[] childs = args[1].Split(new char[] { ',' });
            ulong[] childIDs = new ulong[childs.Length];

            for (int i = 0; i < childs.Length; i++)
            {
                childIDs[i] = ulong.Parse(childs[i]);
                if (!children.Contains(childIDs[i]))
                    children.Add(childIDs[i]);
            }

            foreach (object ID in children)
            {
                try
                {
                    bool match = false;
                    for (int i = 0; i < childIDs.Length; i++)
                        if (childIDs[i] == (ulong)ID) match = true;
                    if (!match) children.Remove(ID);
                }
                catch
                {
                }
            }

            LastCmd = args[0];
        }

        void processData(string[] raw)
        {
            string[] args = raw[1].Split(new char[] { ',' });
            string data = raw[2];
            
            
            return;
        }

        void updateChildren()
        {
            string cmd;
            DateTime start = DateTime.Now;
            cmd = "AT" + CMD_CHILD + "?\r";
            radio.Write(UTF8Encoding.UTF8.GetBytes(cmd), 0, cmd.Length);
            Ack();
            return;
            /*
            if (children.Count == 0) return;

            for (int i = 0; i < children.Count; i++)
            {
                cmd = "AT" + CMD_PING + children[i] + "\r";
                radio.Write(UTF8Encoding.UTF8.GetBytes(cmd), 0, cmd.Length);
                while (!GotResponse)
                {
                    if (DateTime.Now > (start.AddSeconds(30)))
                    {
                        children.RemoveAt(i);
                        if (children.Count == 0) return;
                        i--;
                        break;
                    }
                    Thread.Sleep(100);
                }
            }*/
        }

        void processEvent(string[] args)
        {
            //string[] args = e.Split(new char[] { ' ' });
            switch (args[1])
            {
                case "CHILD_JOINED": // check if the child already exists and if not add him to the list
                    try
                    {
                        ulong ID = ulong.Parse(args[2]);
                        if (children.Contains(ID))
                            return;
                        else
                            children.Add(ID);
                        break;
                    }
                    catch
                    {
                        break;
                    }
                default:
                    break;

            }
            return;
        }

        void radio_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            byte[] _rx = new byte[port.BytesToRead];
            port.Read(_rx, 0, _rx.Length);

            for (int i = 0; i < _rx.Length; i++)
            {
                switch (_rx[i])
                {
                    case (byte)'\r':
                        processCommand();
                        break;
                    case (byte)'\n':
                        break;
                    default:
                        rx[rxcnt++] = _rx[i];
                        if (rxcnt > rx.Length) rxcnt = 0;
                        break;
                }
            }
            
            //throw new NotImplementedException();
        }

        public bool Init()
        {
            config.connected = false;
            bool success = true;
            string cmd;

            //radio init
            success &= Echo(config.echo);
            success &= SetAddrExt(config.addrExtend);
            radio.Write(Encoding.UTF8.GetBytes("AT+WCHMASK=100000\r"), 0, 18);
            success &= Ack();
            success &= SetRole(config.role);
            success &= SetAddrShort(config.addrShort);
            success &= Join(config.panID);
            radio.Write(Encoding.UTF8.GetBytes("ATX\r\n"), 0, 5);
            success &= Ack();
          /*  cmd = "AT" + CMD_RETRY + " " + config.retries + "\r\n";
            radio.Write(Encoding.UTF8.GetBytes(cmd), 0, cmd.Length);
            success &= Ack();*/
            return success;

        }

        public bool CheckStatus()
        {
            string cmd = "AT" + CMD_STATUS + "\r";
            updateChildren();
            radio.DiscardInBuffer();
            radio.Write(Encoding.UTF8.GetBytes(cmd), 0, cmd.Length);
            return Ack();
        }

        public bool Echo(bool on)
        {   
            string cmd;

            if (on)
                cmd = "AT" + CMD_ECHO + "1" + "\r";
            else
                cmd = "AT" + CMD_ECHO + "\r";

            radio.Write(Encoding.UTF8.GetBytes(cmd), 0, cmd.Length);
            return Ack();
        }

        public bool SetPANID(ulong id)
        {
            string cmd = "AT" + CMD_PANID + "=" + id.ToString() + "\r";
            config.panID = id;


            radio.DiscardInBuffer();
            radio.Write(Encoding.UTF8.GetBytes(cmd), 0, cmd.Length);
            return Ack();
        }

        // need to be disconnected from the network to use this command
        public bool SetAddrExt(ulong addr)
        {
            string cmd = "AT" + CMD_ADDRE + "=" + addr.ToString() + "\r";
            config.addrExtend = addr;

            radio.DiscardInBuffer();
            radio.Write(Encoding.UTF8.GetBytes(cmd), 0, cmd.Length);
            return Ack();
        }

        public bool SetAddrShort(int addr)
        {
            string cmd = "AT" + CMD_ADDR + "=" + addr.ToString() + "\r";
            config.addrShort = addr;

            radio.DiscardInBuffer();
            radio.Write(Encoding.UTF8.GetBytes(cmd), 0, cmd.Length);
            return Ack();
        }

        public bool SetRole(Role r)
        {
            string cmd = "AT" + CMD_ROLE + "=" + r.ToString() + "\r";
            config.role = r;

            radio.DiscardInBuffer();
            radio.Write(Encoding.UTF8.GetBytes(cmd), 0, cmd.Length); 
            return Ack();
        }

        public bool Ping(int addr)
        {
            string cmd = "AT" + CMD_PING + " " + addr.ToString();
            radio.Write(Encoding.UTF8.GetBytes(cmd), 0, cmd.Length);
            return Ack();
        }

        // Join a network after setting up all the config parameters
        public bool Join()
        {
            string cmd = "AT" + CMD_JOIN + "\r";
            if(config.connected) Leave();

            radio.DiscardInBuffer();
            radio.Write(Encoding.UTF8.GetBytes(cmd), 0, cmd.Length);
            Thread.Sleep(2000);

            config.connected = Ack();
            return config.connected;
        }

        public bool Join(ulong panID)
        {
            bool success = false;
            if (success = SetPANID(panID))
                success &= Join();
            return success;
        }

        // Leave a network
        public bool Leave()
        {
            config.connected = false;

            radio.DiscardInBuffer();
            radio.Write(Encoding.UTF8.GetBytes("AT+WLEAVE\r"), 0, 10);
            return Ack();
        }

        public bool Ack()
        {
            DateTime start = DateTime.Now;
            while (!GotResponse)
            {
                if (DateTime.Now > start.AddSeconds(5))
                    return false;
            }

            return GotACK;
        }

    }
}
