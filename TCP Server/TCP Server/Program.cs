using System;
using System.Net;
using TCP_Server;

ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

//string[] name = { "Mark", "Markus" };
//VCS Mark = new VCS(name);
//Mark.StartListening();

Server server = new Server();

while (true)
     server.StartListening();

