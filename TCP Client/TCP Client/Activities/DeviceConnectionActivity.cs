using System;
using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Widget;
using System.Net.Sockets;
using Android.Content;
using System.IO;

namespace TCP_Client.Activities
{
    [Activity(Label = "Device menu")]
    public class DeviceConnectionActivity : Activity
    {
        private Button btnAddDevice;
        private EditText edtDeviceName, edtDeviceUrl;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.DeviceConnectionPage);
            btnAddDevice = FindViewById<Button>(Resource.Id.buttonAddConnectionDevice);
            edtDeviceName = FindViewById<EditText>(Resource.Id.editTextDeviceName);
            edtDeviceUrl = FindViewById<EditText>(Resource.Id.editTextDeviceUrl);

            string filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Devices.txt");

            btnAddDevice.Click += async delegate
            {
                try
                {
                    if (edtDeviceName.Text != "" && edtDeviceUrl.Text != "")
                    {
                        string device = edtDeviceName.Text + ':' + edtDeviceUrl.Text + ';';
                        File.AppendAllText(filePath, device);

                        Intent intent = new Intent(this, typeof(Activities.ConectionActivity));
                        StartActivity(intent);
                    }
                }
                catch { }
            };
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}