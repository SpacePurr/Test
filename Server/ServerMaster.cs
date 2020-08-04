using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerMaster
    {
        private UdpClient _udpServer;
        private IPEndPoint _remoteEndPoint;

        private string _multicastIPAddress;
        private int _multicastPort;
        private int _port;

        private int packageIndex;
        private readonly Random _rand = new Random();

        private int _start;
        private int _end;

        public ServerMaster()
        {
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            Settings settings = CommonSerializer.Deserialize<Settings>("settings.xml");
            if (settings != null)
            {
                _multicastPort = settings.MulticastPort;
                _multicastIPAddress = settings.MulticastIPAddress;
                _port = settings.Port;
                _start = settings.Start;
                _end = settings.End;
            }

            packageIndex = 1;
        }

        public async Task Start()
        {
            Console.WriteLine("Starting server...");
            using (_udpServer = new UdpClient(_port, AddressFamily.InterNetwork))
            {
                _remoteEndPoint = new IPEndPoint(IPAddress.Parse(_multicastIPAddress), _multicastPort);

                _udpServer.JoinMulticastGroup(IPAddress.Parse(_multicastIPAddress));
                _udpServer.MulticastLoopback = true;

                Console.WriteLine($"Server started...");

                await Task.Factory.StartNew(SendPackage);
            }
        }

        private void SendPackage()
        {
            while (packageIndex < int.Parse("10e7", NumberStyles.AllowExponent))
                Send($"{packageIndex} : {_rand.Next(_start, _end)}");
        }

        private void Send(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            _udpServer.Send(bytes, bytes.Length, _remoteEndPoint);
            packageIndex++;
        }
    }
}
