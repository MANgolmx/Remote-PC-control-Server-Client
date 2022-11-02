using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using System;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using AndroidX.AppCompat.App;

namespace TCP_Client.Activities
{
    [Activity(Label = "Server controller")]
    public class MainActivity : AppCompatActivity
    {
        //private Button btnTakeScreen, btnSleep;
        //private ImageView imageView;
        private Button btnShutdown, btnDisconnect, btnOpenApp, btnSendMessage;
        private EditText edtTime, edtAppName, edtMessage;
        private Timer checkConnectionTimer;

        NetworkStream stream;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            var client = Connection.Connection.Instance.client;
            client.SendTimeout = 5000;
            client.ReceiveTimeout = 10000;

            checkConnectionTimer = new Timer(1000);
            checkConnectionTimer.Elapsed += CheckConnection;
            checkConnectionTimer.AutoReset = true;
            checkConnectionTimer.Start();

                                //Seting view\\
            //btnTakeScreen = FindViewById<Button>(Resource.Id.buttonTakeScreen);
            //btnSleep = FindViewById<Button>(Resource.Id.buttonSleep);
            //imageView = FindViewById<ImageView>(Resource.Id.imageView);
            SetContentView(Resource.Layout.MainPage);
            btnShutdown = FindViewById<Button>(Resource.Id.buttonShutdown);
            btnDisconnect = FindViewById<Button>(Resource.Id.buttonDisconnect);
            btnOpenApp = FindViewById<Button>(Resource.Id.buttonOpen);
            btnSendMessage = FindViewById<Button>(Resource.Id.buttonSendMsg);
            edtTime = FindViewById<EditText>(Resource.Id.editTextTime);
            edtAppName = FindViewById<EditText>(Resource.Id.editTextApp);
            edtMessage = FindViewById<EditText>(Resource.Id.editTextMsg);

            void CheckConnection(Object source, System.Timers.ElapsedEventArgs e)
            {
                if (client.Client.Poll(100, SelectMode.SelectRead))
                {
                    disconnect(client);
                }
            }

                            //Button clicks\\

            btnShutdown.Click += delegate
            {
                try
                {
                    stream = client.GetStream();
                    String msg = "CMD_SHTD" + edtTime.Text;
                    byte[] message = Encoding.ASCII.GetBytes(msg);
                    stream.Write(message, 0, message.Length);
                }
                catch (Exception e)
                {
                    disconnect(client);
                }
            };

            btnOpenApp.Click += delegate
            {
                if (edtAppName.Text == "" || edtAppName.Text == " ") return;
                try
                {
                    stream = client.GetStream();
                    String msg = "CMD_OPNAP " + edtAppName.Text;
                    byte[] message = Encoding.ASCII.GetBytes(msg);
                    stream.Write(message, 0, message.Length);
                }
                catch (Exception e)
                {
                    disconnect(client);
                }
            };

            btnDisconnect.Click += delegate
            {
                try
                {
                    stream = client.GetStream();
                    String msg = "CMD_LOGOUT";
                    byte[] message = Encoding.ASCII.GetBytes(msg);
                    stream.Write(message, 0, message.Length);
                }
                catch (Exception e) {; }
                finally
                {
                    disconnect(client);
                }
            };

            btnSendMessage.Click += delegate
            {
                try
                {
                    stream = client.GetStream();
                    String msg = "CMD_MSG " + edtMessage.Text;
                    byte[] message = Encoding.ASCII.GetBytes(msg);
                    stream.Write(message, 0, message.Length);
                }
                catch (Exception e) 
                {
                    disconnect(client);
                }
            };

                            //Old functions\\

            /*btnSleep.Click += delegate
            {
                try
                {
                    stream = client.GetStream();
                    String msg = "CMD_SLP";
                    byte[] message = Encoding.ASCII.GetBytes(msg);
                    stream.Write(message, 0, message.Length);
                }
                catch (Exception e)
                {
                    disconnect(client);
                }
            };

            btnTakeScreen.Click += delegate
            {
                try
                {
                    stream = client.GetStream();
                    String msg = "CMD_TSC";
                    byte[] message = Encoding.ASCII.GetBytes(msg);
                    stream.Write(message, 0, message.Length);
                    var data = getData(client);
                    var image = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                    imageView.SetImageBitmap(image);
                }
                catch (Exception e)
                {
                    disconnect(client);
                }
            };*/
        }

        public void disconnect(TcpClient client)
        {
            client.Close();
            StartActivity(typeof(ConectionActivity));
        }

        public byte[] getData(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] fileSizeBytes = new byte[4];
            int bytes = stream.Read(fileSizeBytes, 0, fileSizeBytes.Length);
            int dataLength = BitConverter.ToInt32(fileSizeBytes, 0);

            int bytesLeft = dataLength;
            byte[] data = new byte[dataLength];

            int buffersize = 1024;
            int bytesRead = 0;

            while (bytesLeft > 0)
            {
                int curDataSize = Math.Min(buffersize, bytesLeft);
                if (client.Available < curDataSize)
                    curDataSize = client.Available;//This save me

                bytes = stream.Read(data, bytesRead, curDataSize);
                bytesRead += curDataSize;
                bytesLeft -= curDataSize;
            }
            return data;
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
        }


    }
}