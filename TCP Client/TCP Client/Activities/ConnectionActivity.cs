using System;
using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Widget;
using System.Net.Sockets;
using Android.Content;
using System.IO;

namespace TCP_Client
{
    [Activity(MainLauncher = true)]
    public class ConectionActivity : AppCompatActivity
    {
        private EditText edtIp, edtport;
        private Button btnConnect;
        private TcpClient client;
        ActionBar actionBar = new Android.App.ActionBar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            client = new TcpClient();

            SetContentView(Resource.Layout.ConnectionPage);
            
            edtIp = FindViewById<EditText>(Resource.Id.editTextIP);
            edtport = FindViewById<EditText>(Resource.Id.editTextPort);
            btnConnect = FindViewById<Button>(Resource.Id.buttonConnect);
            
            string filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "ConnectionIP.txt");
            string ConnectionIp;

            if (File.Exists(filePath)) {
                ConnectionIp = File.ReadAllText(filePath);
                edtIp.Text = ConnectionIp;
            }

            btnConnect.Click += async delegate
            {
                try
                {
                    await client.ConnectAsync(edtIp.Text, Convert.ToInt32(edtport.Text));
                    if (client.Connected)
                    {
                        ConnectionIp = edtIp.Text;
                        File.WriteAllText(filePath, ConnectionIp);

                        Connection.Connection.Instance.client = client;
                        Toast.MakeText(this, "Client connected to server!", ToastLength.Short).Show();
                        Intent intent = new Intent(this, typeof(Activities.MainActivity));
                        StartActivity(intent);
                    }
                    else
                    {
                        Toast.MakeText(this, "Connection failed!", ToastLength.Short).Show();
                    }
                }
                catch (Exception x)
                {
                    Toast.MakeText(this, "Connection failed!", ToastLength.Short).Show();
                    Toast.MakeText(this, "" + x, ToastLength.Short).Show();
                }
            };

        }

	}
}
