
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class ChatClient
    {
        IPAddress ip;
        int port;
        TcpClient client;

        public ChatClient(string endPoint)
        {
            InitAddress(endPoint);
            var addressFamily = AddressFamily.InterNetwork;
            client = new TcpClient(addressFamily);
        }
       
        void InitAddress(string endPoint)
        {
            try
            {
                var parts = endPoint.Split(':');
                ip = IPAddress.Parse(parts[0]);
                port = int.Parse(parts[1]);
            }
            catch
            {
                Console.WriteLine("Wrong address!");
            }
        }

        public void Process()
        {
            try
            {
                client.ConnectAsync(ip, port);
                Console.WriteLine("Connected.");
                Task.Run(() => ReadProcess());
                Console.WriteLine("Read process started.");
                var stream = client.GetStream();
                while( true )
                { 
                    var message = Console.ReadLine();
                    var writeBuffer = Encoding.UTF8.GetBytes(message);
                    stream.Write(writeBuffer, 0, writeBuffer.Length);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }

        async void ReadProcess()
        {
            var stream = client.GetStream();
            while( true )
            {
                try
                {
                    if( stream.DataAvailable )
                    {
                        var readBuffer = new byte[256];
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
                            Console.WriteLine(message);
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                    break;
                }
            }
        }
    }
}