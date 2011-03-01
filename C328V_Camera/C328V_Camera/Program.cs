using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO.Ports;
using GHIElectronics.NETMF.FEZ;

using CameraApp30;
using DSS.Devices;

namespace C328V_Camera
{
    public class Program
    {
        static CameraApp30.C328R camera;
        static uOLED disp;

        static Boolean _ack = false;

        static Boolean ack
        {
            get
            {
                Boolean prev = _ack;
                _ack = false;

                return prev;
            }

            set
            {
                _ack = value;
            }
        }

   
        static OutputPort led;
  


        public static void Main()
        {
        
            SerialPort d = new SerialPort("COM2", 9600);
            d.Open();

            bool ledState = false;

            led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);

            camera = new C328R(new SerialPort("COM1", 115200));

            InitCamera();
            disp = new uOLED(d);
            
            //disp.ShutDown(false);
      


            // Picture data buffer
            byte[] pictureData;

            // Get instant Jpeg picture - give some process delay
            //camera.GetJpegPicture(C328R.PictureType.Jpeg, out pictureData, 800);
            disp.Cls();
            Random r = new Random();
            while (true)
            {
               
                disp.Cls();
                camera.Snapshot(C328R.SnapshoteType.Uncompressed, 0);
                Thread.Sleep(1000);

                //camera.GetJpegPicture(C328R.PictureType.Jpeg, out pictureData, 800);
                camera.GetRawPicture(C328R.PictureType.Snapshot, out pictureData, 800);
                
                if(pictureData.Length != 0)
                    disp.DrawImage(30, 30, 160, 120, 0x08, pictureData);
                //InitCamera();

                disp.SetBackground(new byte[] { (byte)r.Next(255), (byte)r.Next(255) });
                //disp.DrawRectangle((Byte)r.Next(255), (Byte)r.Next(255), (Byte)r.Next(255), (Byte)r.Next(255), (Byte)r.Next(255), (Byte)r.Next(255), (Byte)r.Next(255), (Byte)r.Next(255), new byte[] {0xFF, 0xFF});
                Thread.Sleep(500);
            }
            
        }

        private static void InitCamera()
        {
            // Synchronize with camera
            Debug.Print(camera.Sync().ToString());

            // Set baud rate - optional
           camera.SetBaudRate(C328R.BaudRate.Baud38400);
            // Set light frequency - optional
            //camera.LigtFrequency(C328R.FrequencyType.F50Hz);

            // Initiate camera and picture details
            Debug.Print(camera.Initial(C328R.ColorType.GreyScale8, C328R.PreviewResolution.R160x120, C328R.JpegResolution.R160x128).ToString());
        }
        
    }
}
