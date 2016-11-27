using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;

namespace ChatServer 
{
    public class Server 
    {
        TcpListener listener;
        List<ChatClientHolder> clients = new List<ChatClientHolder>();

        public Server(int port) 
        {
            var address = IPAddress.Any;
            listener = new TcpListener(address, port);
            Console.WriteLine($"Server inited on {address}:{port}");
        }

        public void Process() {
            try 
            {
                listener.Start();
                while( true )
                {
                    var tcpClient = listener.AcceptTcpClientAsync();
                    tcpClient.ContinueWith(ProcessClient);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception in Process: {e.Message}");
            }
        }

        void ProcessClient(Task<TcpClient> task) {
            var client = task.Result;
            var index = 0;
            lock( clients ) 
            {
                index = clients.Count;
            }
            var holder = new ChatClientHolder(client, index);
            OnClientConnected(holder);
            var stream = client.GetStream();
            var readBuffer = new byte[1024];
            while( true ) {
                if( !IsClientConnected(client) ) 
                {
                    break;
                }
                try
                {
                    var messageBuilder = new StringBuilder();
                    int readBytes = 0; 
                    do{
                        readBytes = stream.Read(readBuffer, 0, readBuffer.Length);
                        messageBuilder.AppendFormat("{0}", Encoding.UTF8.GetString(readBuffer, 0, readBytes));
                    }
                    while(stream.DataAvailable);
                    if( messageBuilder.Length > 0 ) 
                    {
                        var message = messageBuilder.ToString().TrimEnd();
                        message = $"{holder.Name}: \"{message}\"{Environment.NewLine}";
                        Console.WriteLine(message);
                        BroadcastMessage(message, holder);
                    }
                } 
                catch (Exception e)
                {
                    Console.WriteLine($"Exception in ProcessClient: {e.Message}");
                    break;
                } 
            }
            OnClientDisconnected(holder);
            client.Dispose();
        }

        bool IsClientConnected(TcpClient client) {
            try 
            {
                if( client.Client.Poll( 0, SelectMode.SelectRead ) )
                {
                    byte[] buff = new byte[1];
                    if( client.Client.Receive( buff, SocketFlags.Peek ) == 0 )
                    {
                        return false;
                    }
                }
            } 
            catch (Exception e) 
            {
                Console.WriteLine($"Exception in IsClientConnected: {e.Message}");
                return false;
            }
            return true;
        }

        void OnClientConnected(ChatClientHolder client)
        {
            var message = $"Client connected: {client.Name}.{Environment.NewLine}";
            Console.Write(message);
            BroadcastMessage(message);
            clients.Add(client);
        }

        void OnClientDisconnected(ChatClientHolder client)
        {
            var message = $"Client disconnected: {client.Name}.{Environment.NewLine}";
            clients.Remove(client);
            Console.Write(message);
            BroadcastMessage(message);
        }

        void BroadcastMessage(string message, ChatClientHolder owner = null) 
        {
            lock(clients)
            {
                byte[] writeBuffer = Encoding.UTF8.GetBytes(message);
                foreach( var client in clients )
                {
                    if( (owner == null) || (owner != client) ) 
                    { 
                        var stream = client.Client.GetStream();
                        stream.Write(writeBuffer, 0, writeBuffer.Length);
                    }
                }
            }
        }
    }   
}