using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PCKontolServer
{
    public class Test
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;
        private const int APPCOMMAND_MEDIA_PLAY_PAUSE = 0xE00000;
        IntPtr Handle = Process.GetCurrentProcess().MainWindowHandle;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        public void Mute()
        {
            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
                (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }

        public void VolDown()
        {
            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
                (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }

        public void VolUp()
        {
            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
                (IntPtr)APPCOMMAND_VOLUME_UP);
        }
        public void PPause()
        {
            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
                (IntPtr)APPCOMMAND_MEDIA_PLAY_PAUSE);
        }
    }
    class Server
    {
        private static TcpListener listener { get; set; }
        private static bool accept { get; set; } = false;

        public static void StartServer(int port)
        {
            Console.WriteLine("Ip Adresini Giriniz:");
            IPAddress address = IPAddress.Parse(Console.ReadLine());
            listener = new TcpListener(address, port);

            listener.Start();
            accept = true;

            Console.WriteLine($"Server Baslatildi, Port=" + port);
        }

        public static void Listen()
        {
            Test tus = new Test();
            if (listener != null && accept)
            {

                // Continue listening.  
                while (true)
                {
                    Console.WriteLine("Baglanti Bekleniyor..");
                    var clientTask = listener.AcceptTcpClientAsync(); // Get the client  

                    if (clientTask.Result != null)
                    {
                        Console.WriteLine("Baglanti tamamlandi, Veriler bekleniyor.");
                        var client = clientTask.Result;
                        Random pinkodu = new Random();
                        string message = "";
                        //pkontrol:
                        int pin = pinkodu.Next(1000,9999);
                        Boolean a=true;
                        Console.WriteLine("Pin kodu =" + pin);
                        while (message != null && !message.StartsWith("quit"))
                        {
                            byte[] data = Encoding.ASCII.GetBytes("Send next data: [enter 'quit' to terminate] ");
                            client.GetStream().Write(data, 0, data.Length);

                            byte[] buffer = new byte[1024];
                            client.GetStream().Read(buffer, 0, buffer.Length);

                            message = Encoding.ASCII.GetString(buffer);
                            Console.WriteLine(message);
                            if (a)
                            {
                                if(pin == Int32.Parse(message.Substring(0,4)))
                                {
                                    a=false;
                                }
                                else
                                {
                                    pin=pinkodu.Next(1000,9999);
                                    Console.WriteLine("Pin kodu =" + pin);
                                }
                            }
                            else
                            {
                                if (message.StartsWith("volumeup"))
                                {
                                    tus.VolUp();
                                }
                                else if (message.StartsWith("volumedown"))
                                {
                                    tus.VolDown();
                                }
                                else if (message.StartsWith("mute"))
                                {
                                    tus.Mute();
                                }
                                else if (message.StartsWith("ppause"))
                                {
                                    tus.PPause();
                                }
                            }
                        }
                        Console.WriteLine("Closing connection.");
                        client.GetStream().Dispose();
                    }
                }
            }
        }
    }
}
