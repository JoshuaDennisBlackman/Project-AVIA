using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkServer
{

    class Program
    {
        static void Main(string[] args)
        {
            Server s = new Server();
        }

    }

    class Server
    {
        private TcpListener tcpListener;
        private Thread listenThread;

        public Server()
        {
            Console.WriteLine(@"╔═╗╦═╗╔═╗ ╦╔═╗╔═╗╔╦╗  ╔═╗╦  ╦╦╔═╗  ╔═╗╔═╗╦═╗╦  ╦╔═╗╦═╗");
            Console.WriteLine(@"╠═╝╠╦╝║ ║ ║║╣ ║   ║   ╠═╣╚╗╔╝║╠═╣  ╚═╗║╣ ╠╦╝╚╗╔╝║╣ ╠╦╝");
            Console.WriteLine(@"╩  ╩╚═╚═╝╚╝╚═╝╚═╝ ╩   ╩ ╩ ╚╝ ╩╩ ╩  ╚═╝╚═╝╩╚═ ╚╝ ╚═╝╩╚═");
            w(ConsoleColor.Cyan, "Initializing Server...");
            w(ConsoleColor.Cyan, "Connecting to Database...");
            this.tcpListener = new TcpListener(IPAddress.Any, 3000);
            w(ConsoleColor.Cyan, "Listening for clients on port 3000...");
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();

                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            DatabaseInterface db = new DatabaseInterface("credentials.txt");
            byte[] message = new byte[4096];
            int bytesRead;
            w(ConsoleColor.Green, "Inbound connection from " + tcpClient.Client.RemoteEndPoint + ".");
            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    w(ConsoleColor.Yellow, encoder.GetString(message, 0, bytesRead));
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }
            }

            w(ConsoleColor.Red, tcpClient.Client.RemoteEndPoint + " has disconnected from the server.");
            tcpClient.Close();
            
        }

        public void w(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[" + DateTime.Now.ToString("h:mm:ss tt") + "] >> " + text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

    }
}
