using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HW14
{

    public class ChatClient
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private StreamReader reader;
        private StreamWriter writer;
        private string username;
        private UdpClient udpClient;
        private string serverIp = "127.0.0.1";
        private int serverPort = 8888;

        public ChatClient()
        {
            tcpClient = new TcpClient();
            udpClient = new UdpClient();
        }

        public void Start()
        {
            ConnectToServer();
            Thread listenThread = new Thread(new ThreadStart(ListenForMessages));
            listenThread.Start();
            while (true)
            {
                string message = Console.ReadLine();
                if (message.StartsWith("/discover"))
                {
                    DiscoverServers();
                }
                else
                {
                    SendMessageToServer(message);
                }
            }
        }

        private void ConnectToServer()
        {
            tcpClient.Connect(serverIp, serverPort);
            networkStream = tcpClient.GetStream();
            reader = new StreamReader(networkStream);
            writer = new StreamWriter(networkStream);

            Console.WriteLine("Enter your username: ");
            username = Console.ReadLine();
            writer.WriteLine(username);
            writer.Flush();
        }

        private void ListenForMessages()
        {
            string message;
            while ((message = reader.ReadLine()) != null)
            {
                Console.WriteLine(message);
            }
        }

        private void SendMessageToServer(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }

        private void DiscoverServers()
        {
            udpClient.Send(Encoding.ASCII.GetBytes("DISCOVER_SERVER"), "127.0.0.1", 12345);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 12345);
            byte[] receivedData = udpClient.Receive(ref remoteEndPoint);
            Console.WriteLine($"Server discovered: {Encoding.ASCII.GetString(receivedData)}");
        }
    }

}

