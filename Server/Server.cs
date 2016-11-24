using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;

namespace ChatServer 
{
    public class Server 
    {
        TcpListener listener;
        public Server(int port) 
        {
            var address = IPAddress.Any;
            listener = new TcpListener(address, port);
            Console.WriteLine($"Server inited on {address}:{port}");
        }

        public void Process() {
            listener.Start();
            while( true )
            {
                var tcpClient = listener.AcceptTcpClientAsync();
                tcpClient.ContinueWith(ProcessClient);
            }
        }

        void ProcessClient(Task<TcpClient> task) {
            Console.WriteLine("Client connected");
            var client = task.Result;
            var stream = client.GetStream();
            var readBuffer = new byte[1024];
            var messageBuilder = new StringBuilder();
            int readBytes = 0; 
            do{
                readBytes = stream.Read(readBuffer, 0, readBuffer.Length);
                messageBuilder.AppendFormat("{0}", Encoding.UTF8.GetString(readBuffer, 0, readBytes));
            }
            while(stream.DataAvailable);
            var message = messageBuilder.ToString();
            Console.WriteLine($"Client message: \"{message}\"");
            client.Dispose();
        }
    }   
}