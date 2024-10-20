namespace HW14;

class Program
{
    static void Main(string[] args)
    {
        ChatServer server = new ChatServer("127.0.0.1", 8888);
        server.Start();

    }
}

