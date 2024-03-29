﻿using System;
using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Widget;
using System.Net.Sockets;
using Android.Content;
using System.IO;

namespace TCP_Client.Activities
{
    [Activity(MainLauncher = true)]
    public class ConectionActivity : AppCompatActivity
    {
        private EditText edtIp, edtPort;
        private Button btnConnect, btnAddDevice, btnShowDevices;
        private TcpClient client;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            client = new TcpClient();

            SetContentView(Resource.Layout.ConnectionPage);
            
            edtIp = FindViewById<EditText>(Resource.Id.editTextIP);
            edtPort = FindViewById<EditText>(Resource.Id.editTextPort);
            btnConnect = FindViewById<Button>(Resource.Id.buttonConnect);
            btnAddDevice = FindViewById<Button>(Resource.Id.buttonAddDevice);
            btnShowDevices = FindViewById<Button>(Resource.Id.buttonShowAllDevices);

            string filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "ConnectionIP.txt");
            string ConnectionIp;

            if (File.Exists(filePath)) {
                ConnectionIp = File.ReadAllText(filePath);

                var ip = ConnectionIp.Split(':');
                edtIp.Text = ip[0];
                edtPort.Text = ip[1];
            }

            btnConnect.Click += async delegate
            {
                try
                {
                    await client.ConnectAsync(edtIp.Text, Convert.ToInt32(edtPort.Text));
                    if (client.Connected)
                    {
                        ConnectionIp = edtIp.Text + ':' + edtPort.Text;
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

            btnAddDevice.Click += delegate
            {
                Intent intent = new Intent(this, typeof(Activities.DeviceConnectionActivity));
                StartActivity(intent);
            };

            btnShowDevices.Click += delegate
            {
                Intent intent = new Intent(this, typeof(Activities.DeviceMenuActivity));
                StartActivity(intent);
            };
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
        }

    }
}
