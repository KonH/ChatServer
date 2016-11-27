namespace ChatServer 
{
    public class Program 
    {
        public static void Main(string[] args) 
        {
            var port = 80;
            var server = new Server(port);
            server.Process();
        }
    }
}
