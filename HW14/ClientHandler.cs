using System.Net.Sockets;

namespace HW14
{
    public class ClientHandler
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private StreamReader reader;
        private StreamWriter writer;
        private ChatServer server;
        private string username;

        public ClientHandler(TcpClient tcpClient, ChatServer server)
        {
            this.tcpClient = tcpClient;
            this.server = server;
            networkStream = tcpClient.GetStream();
            reader = new StreamReader(networkStream);
            writer = new StreamWriter(networkStream);
        }

        public void HandleClient()
        {
            // Вхід / Реєстрація
            writer.WriteLine("Enter your username: ");
            writer.Flush();
            username = reader.ReadLine();
            writer.WriteLine($"Welcome {username}!");
            writer.Flush();

            string message;
            while ((message = reader.ReadLine()) != null)
            {
                if (message.StartsWith("/private"))
                {
                    string[] parts = message.Split(' ', 3);
                    string recipient = parts[1];
                    string privateMessage = parts[2];
                    SendPrivateMessage(recipient, privateMessage);
                }
                else if (message == "/quit")
                {
                    break;
                }
                else
                {
                    server.BroadcastMessage($"{username}: {message}", this);
                    server.SaveMessageToHistory($"{username}: {message}");
                }
            }

            // Завершення з'єднання
            tcpClient.Close();
        }

        public void SendMessage(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }

        private void SendPrivateMessage(string recipient, string message)
        {
            foreach (var client in server.clients)
            {
                if (client.username == recipient)
                {
                    client.SendMessage($"Private message from {username}: {message}");
                    return;
                }
            }

            SendMessage($"User {recipient} not found.");
        }
    }

}

