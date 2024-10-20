using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HW14
{

    public class ChatServer
    {
        private TcpListener tcpListener;
        public List<ClientHandler> clients = new List<ClientHandler>();
        private string historyFile = "history.txt";
        private UdpClient udpListener;
        private List<IPEndPoint> knownServers = new List<IPEndPoint>();

        public ChatServer(string ipAddress, int port)
        {
            tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            udpListener = new UdpClient(12345); // порт для пошуку серверів
        }

        public void Start()
        {
            tcpListener.Start();
            Console.WriteLine("Server started...");

            Thread udpThread = new Thread(new ThreadStart(DiscoverServers));
            udpThread.Start();

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                ClientHandler clientHandler = new ClientHandler(tcpClient, this);
                Thread clientThread = new Thread(new ThreadStart(clientHandler.HandleClient));
                clientThread.Start();
                clients.Add(clientHandler);
            }
        }

        public void BroadcastMessage(string message, ClientHandler sender)
        {
            foreach (var client in clients)
            {
                if (client != sender)
                {
                    client.SendMessage(message);
                }
            }
        }

        public void SaveMessageToHistory(string message)
        {
            File.AppendAllText(historyFile, message + Environment.NewLine);
        }

        private void DiscoverServers()
        {
            while (true)
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 12345);
                byte[] data = udpListener.Receive(ref endPoint);
                string message = Encoding.ASCII.GetString(data);
                Console.WriteLine($"Discovered server: {endPoint.Address}");
                knownServers.Add(endPoint);
            }
        }

    }
}

