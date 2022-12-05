using System;
using System.Net;
using TCP_Server;

//string[] name = { "Mark", "Markus" };
//VCS Mark = new VCS(name);
//Mark.StartListening();

public static class Program
{
    public static void Main(string[] args)
    {
        Server server = new Server();

        while (true)
            server.StartListening();
    }
}