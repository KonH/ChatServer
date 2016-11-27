using System;

namespace ChatClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if( args.Length > 0 ) 
            {
                var addressStr = args[0];
                var client = new ChatClient(addressStr);
                client.Process();
            }
            else
            {
                Console.WriteLine("No address provided.");
            }
        }
    }
}
