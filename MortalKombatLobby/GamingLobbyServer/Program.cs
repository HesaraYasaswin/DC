using System;
using System.ServiceModel;
using LobbyCommon;

namespace GamingLobbyServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(GamingLobbyServer));
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = 65536 * 1024; // Increase the message size to 64 MB

            host.AddServiceEndpoint(typeof(IGamingLobby), binding, "net.tcp://localhost:8100/GamingLobbyService");

            host.Open();
            Console.WriteLine("Gaming Lobby Server is running...");
            Console.WriteLine("Press Enter to close the server.");
            Console.ReadLine();
            host.Close();
        }
    }
}
