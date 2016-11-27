using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class ChatClientHolder
    {
        public TcpClient Client { get; }
        public Socket Socket { get; }
        public IPAddress IP { get; }
        public string Name { get; }

        public ChatClientHolder(TcpClient client, int index)
        {
            Client = client;
            Socket = client.Client;
            IP = (client.Client.RemoteEndPoint as IPEndPoint)?.Address;
            Name = IP.ToString() + ((index > 0) ? $"[{index}]" : "");
        }
    }
}