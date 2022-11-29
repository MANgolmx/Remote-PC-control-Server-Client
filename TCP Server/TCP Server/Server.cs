using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Drawing;
using Microsoft.Toolkit.Uwp.Notifications;

namespace TCP_Server
{
    internal class Server
    {
        public static TcpClient client = new TcpClient();

        private static TcpListener listener;
        private static string ipString;
        private static System.Timers.Timer checkConnectionTimer;
        private static System.Timers.Timer ShutdownTimer;
        IPEndPoint ep;

        public Server()
        {
            checkConnectionTimer = new System.Timers.Timer(1000);
            checkConnectionTimer.Elapsed += CheckConnection;
            checkConnectionTimer.AutoReset = true;

            ShutdownTimer = new System.Timers.Timer();
            ShutdownTimer.Elapsed += Shutdown;
            checkConnectionTimer.AutoReset = false;

            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIp)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && address.ToString().Contains("192."))
                {
                    ipString = address.ToString();
                    break;
                } else if (address.AddressFamily == AddressFamily.InterNetwork && ipString != "")
                { ipString = address.ToString(); }
            }

            ParseIpString();
            listener = new TcpListener(ep);

            new ToastContentBuilder().AddText("Server started at " + ep.ToString()).Show();
        }

        private void TransferingData()
        {
            while (client.Connected)
            {
                try
                {
                    const int bytesize = 1024 * 1024;
                    byte[] buffer = new byte[bytesize];
                    string x = client.GetStream().Read(buffer, 0, bytesize).ToString();
                    var data = ASCIIEncoding.ASCII.GetString(buffer);

                    if (client.GetStream() == null) Console.WriteLine("Stream is null");

                    if (data.ToUpper().Contains("CMD_MSG"))
                    {
                        data = data.Replace("CMD_MSG", "");
                        data = data.Replace("\0", "");   
                        new ToastContentBuilder().AddText(data).AddAttributionText("Server received message").Show();
                    }
                    else if (data.ToUpper().Contains("CMD_LOGOUT"))
                    {
                        Console.WriteLine("Client disconnected! \n");

                        client.Close();

                        ClientConnection();
                        continue;
                    }
                    else if (data.ToUpper().Contains("CMD_SHTD"))
                    {
                        Console.WriteLine("Pc is going to Shutdown!");
                        data = data.Replace("CMD_SHTD4", "");
                        data = new String(data.Where(Char.IsDigit).ToArray());
                        if (data == "" || data == "0") data = "1";
                        {
                            if (Int32.Parse(data) <= 30) Shutdown(Int32.Parse(data));
                            else
                            {
                                ShutdownTimer.Interval = Int32.Parse(data) * 1000 - 30000;
                                ShutdownTimer.Enabled = true;
                            }
                            new ToastContentBuilder().AddText("Computer will be shutted down in " + ParseTime(Int32.Parse(data))).Show();
                        }
                    }
                    else if (data.ToUpper().Contains("CMD_OPNAP"))
                    {
                        Console.Write("Opening app ");
                        data = data.Replace("CMD_OPNAP ", "");
                        data = new String(data.Where(Char.IsLetterOrDigit).ToArray());
                        Console.WriteLine(data.ToUpper());

                        var allFiles = Directory.GetFiles("C:\\Programs", data.ToUpper() + ".lnk");
                        foreach (var file in allFiles)
                        {
                            Process app = new Process();
                            app.StartInfo.UseShellExecute = true;
                            app.StartInfo.WorkingDirectory = "C:\\Programs";
                            app.StartInfo.FileName = data.ToUpper() + ".lnk";
                            app.Start();
                        }
                    }
                    else if (data.ToUpper().Contains("CMD_SLP"))
                    {
                        Console.WriteLine("Sleep doesn't work!");
                        //Sleep();
                    }
                    else if (data.ToUpper().Contains("CMD_TSC"))
                    {
                        Console.WriteLine("Screenshots does not work");

                        //Bitmap bitmap = SaveScreenshot();
                        //MemoryStream stream = new MemoryStream();
                        //bitmap.Save(stream, ImageFormat.Png);
                        //sendData(stream.ToArray(), client.GetStream());
                        //Console.WriteLine("Screenshot sent"); 
                    }
                }
                catch (Exception exc)
                {
                    new ToastContentBuilder().AddText("Error occured: " + exc.Message).AddAttributionText("Client was disconnected").Show();
                    
                    Console.WriteLine("Client was disconnected because of an error!");
                    Console.WriteLine(exc.Message + "\n");

                    client.Close();

                    Console.WriteLine("Started listening");
                    ClientConnection();
                    continue;
                }
            }
        }

        private void CheckConnection(Object source, ElapsedEventArgs e)
        {
            if (client.Client.Poll(100, SelectMode.SelectRead))
            {
                Console.WriteLine("Client disconnected! \n");

                client.Close();

                ClientConnection();
            }
        }

        public void StartListening()
        {
            if (listener != null)
            {
                listener.Start();

                Console.WriteLine(@"  
            ===============================================================  
                   Started listening requests at: {0}:{1}  
            ===============================================================",
            ep.Address, ep.Port);
            }

            ClientConnection();
        }

        private void ClientConnection()
        {
            checkConnectionTimer.Enabled = false;
            
            client = listener.AcceptTcpClient();
            checkConnectionTimer.Enabled = true;
            Console.WriteLine("Client connected!");

            TransferingData();
        }
        
        private void ParseIpString()
        {
            try
            {
                ep = new IPEndPoint(IPAddress.Parse(ipString), 13031);
            }
            catch
            {
                new ToastContentBuilder().AddText("Can not open server on " + ipString + ":13031").AddAttributionText("Server closed").Show();
                Environment.Exit(404);
            }
        }

        private void Shutdown(Object source, ElapsedEventArgs e)
        {
            Process.Start("Shutdown", "-s -t 30");
            Environment.Exit(0);
        }

        private void Shutdown(int delay)
        {
            Process.Start("Shutdown", "-s -t " + delay);
            Environment.Exit(0);
        }

        private string ParseTime(double seconds)
        {
            Console.WriteLine(seconds.ToString());

            seconds = Math.Floor(seconds);

            //Error check
            if (seconds < 0) return "Error: negative time. " + seconds;

            //Under a minute
            if (seconds < 60) return seconds + "s.";

            //Under an hour
            if (seconds < 3600)
            {
                if (seconds % 60 == 0) return (seconds / 60) + "m.";
                return Math.Floor(seconds / 60) + "m " + (seconds % 60) + "s.";
            }

            //The rest
            if (seconds % 3600 == 0) return (seconds / 3600) + "h.";
            if (seconds % 60 == 0) return Math.Floor(seconds / 3600) + "h " + Math.Floor((seconds % 3600) / 60) + "m.";
            return Math.Floor(seconds / 3600) + "h " + Math.Floor((seconds % 3600) / 60) + "m " + (seconds % 60) + "s.";
        }

        //Old functions
        private void Sleep()
        {
            
        }
        private void sendData(byte[] data, NetworkStream stream)
        {
            int bufferSize = 1024;

            byte[] dataLength = BitConverter.GetBytes(data.Length);

            stream.Write(dataLength, 0, 4);

            int bytesSent = 0;
            int bytesLeft = data.Length;

            while (bytesLeft > 0)
            {
                int curDataSize = Math.Min(bufferSize, bytesLeft);

                stream.Write(data, bytesSent, curDataSize);

                bytesSent += curDataSize;
                bytesLeft -= curDataSize;
            }
        }
        private Bitmap SaveScreenshot()
        {
            using var bmpScreenshot = new Bitmap(1920, 1080);

            using (var g = Graphics.FromImage(bmpScreenshot))
            {
                g.CopyFromScreen(0, 0, 0, 0,
                bmpScreenshot.Size, CopyPixelOperation.SourceCopy);
            }

            //bmpScreenshot.Save("filename.jpg", ImageFormat.Bmp);

            return bmpScreenshot;
        }
    }
}
