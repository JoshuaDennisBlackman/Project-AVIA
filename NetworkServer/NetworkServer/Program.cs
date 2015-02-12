using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data;

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
            w(ConsoleColor.Cyan, "Testing Database Connection");
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
                    string input = encoder.GetString(message, 0, bytesRead);


                    if(input == "meeting"){
                        DataTable data = db.ExecuteQuery("SELECT id FROM  `meetings` LIMIT 0 , 30");
                        w(ConsoleColor.Yellow, tcpClient.Client.RemoteEndPoint + " requested the meetings table.");
                        if (data != null)
                        {
                            foreach (DataRow row in data.Rows)
                            {
                                foreach (var item in row.ItemArray)
                                {
                                    string jsonString = item.ToString();
                                    byte[] id = new byte[1] { 0x32 };
                                    byte[] json = encoder.GetBytes(jsonString);
                                    byte[] l = BitConverter.GetBytes(1 + json.Length);
                                    byte[] buffer = new byte[5 + json.Length];
                                    Buffer.BlockCopy(l, 0, buffer, 0, 4);
                                    Buffer.BlockCopy(id, 0, buffer, 4, 1);
                                    Buffer.BlockCopy(json, 0, buffer, 5, json.Length);
                                    clientStream.Write(buffer, 0, buffer.Length);
                                    clientStream.Flush();
                                }
                            }
                        }
                    } else if(input == "book"){ 
                        bool status = db.BookMeeting("2","test","3","test","test","test","test","test","test");
                        if (status == false)
                        {
                            string jsonString = "Meeting room x failed to be booked";
                            byte[] id = new byte[1] { 0x32 };
                            byte[] json = encoder.GetBytes(jsonString);
                            byte[] l = BitConverter.GetBytes(1 + json.Length);
                            byte[] buffer = new byte[5 + json.Length];
                            Buffer.BlockCopy(l, 0, buffer, 0, 4);
                            Buffer.BlockCopy(id, 0, buffer, 4, 1);
                            Buffer.BlockCopy(json, 0, buffer, 5, json.Length);
                            clientStream.Write(buffer, 0, buffer.Length);
                            clientStream.Flush();
                        }
                        else if(status == true)
                        {
                            string jsonString = "Meeting room x has been booked";
                            byte[] id = new byte[1] { 0x32 };
                            byte[] json = encoder.GetBytes(jsonString);
                            byte[] l = BitConverter.GetBytes(1 + json.Length);
                            byte[] buffer = new byte[5 + json.Length];
                            Buffer.BlockCopy(l, 0, buffer, 0, 4);
                            Buffer.BlockCopy(id, 0, buffer, 4, 1);
                            Buffer.BlockCopy(json, 0, buffer, 5, json.Length);
                            clientStream.Write(buffer, 0, buffer.Length);
                            clientStream.Flush();
                        }
                    }


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
