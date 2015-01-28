using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;

namespace NetworkClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"╔═╗╦═╗╔═╗ ╦╔═╗╔═╗╔╦╗  ╔═╗╦  ╦╦╔═╗  ╔═╗╦  ╦╔═╗╔╗╔╦╗");
            Console.WriteLine(@"╠═╝╠╦╝║ ║ ║║╣ ║   ║   ╠═╣╚╗╔╝║╠═╣  ║  ║  ║║╣ ║║║║ ");
            Console.WriteLine(@"╩  ╩╚═╚═╝╚╝╚═╝╚═╝ ╩   ╩ ╩ ╚╝ ╩╩ ╩  ╚═╝╩═╝╩╚═╝╝╚╝╩");
           

            byte[] message = new byte[4096];
            int bytesRead;

            TcpClient client = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
            client.Connect(serverEndPoint);
            Console.WriteLine("[CLIENT] Connected to server.");
            NetworkStream clientStream = client.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();

            while (true)
            {
                string input = null;
                input = Console.ReadLine();
                if (input == "meeting")
                {
                    byte[] buffer = encoder.GetBytes(input);
                    clientStream.Write(buffer, 0, buffer.Length);
                    BinaryReader br = new BinaryReader(clientStream);
                    int packetSize = BitConverter.ToInt32(br.ReadBytes(5), 0);
                    message = new byte[packetSize];
                    bytesRead = clientStream.Read(message, 0, message.Length);
                    string data = encoder.GetString(message, 0, bytesRead); 
                    Console.WriteLine(data);

                }
                else if (input == "book")
                {
                    byte[] buffer = encoder.GetBytes(input);
                    clientStream.Write(buffer, 0, buffer.Length);
                    BinaryReader br = new BinaryReader(clientStream);
                    int packetSize = BitConverter.ToInt32(br.ReadBytes(5), 0);
                    message = new byte[packetSize];
                    bytesRead = clientStream.Read(message, 0, message.Length);
                    string data = encoder.GetString(message, 0, bytesRead);
                    Console.WriteLine(data);
                }
                else if (input == "quit")
                {
                    break;
                }
            }
            clientStream.Flush();
        }
    }
}
