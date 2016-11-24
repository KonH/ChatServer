namespace ChatServer 
{
    public class Program 
    {
        public static void Main(string[] args) 
        {
            var server = new Server(80);
            server.Process();
        }
    }
}
