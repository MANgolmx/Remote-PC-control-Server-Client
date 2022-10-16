using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TCP_Client
{
    public partial class MainPage : ContentPage
    {
        private TcpClient client;

        public MainPage()
        {
            InitializeComponent();

            client = new TcpClient();
        }

        void ButtonConnectPressed(object sender, EventArgs args)
        {
            client.Connect(IPAddress.Parse(ServerIp.Text), Convert.ToInt32(ServerPort.Text));
        }
    }
}
