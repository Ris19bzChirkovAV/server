using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    class Server
    {
        public TcpListener Listener;
        public List<ClientInfo> clients = new List<ClientInfo>();
        public List<ClientInfo> newClients = new List<ClientInfo>();
        public static Server server;
        static System.IO.TextWriter Out;
        public string incomingMessage;
        public string outMessage = "";

        public Server(int port, System.IO.TextWriter _Out)
        {
            Out = _Out;
            Server.server = this;

            Listener = new TcpListener(IPAddress.Any, port);
            Listener.Start();
        }

        public void begin()
        {
            Thread clientListener = new Thread(ListenerClients);
            clientListener.Start();
            while (true)
            {
                foreach(ClientInfo client in clients)
                {
                    if (client.isConnect)
                    {
                        NetworkStream stream = client.Client.GetStream();
                        while (stream.DataAvailable)
                        {
                            int ReadByte = stream.ReadByte();
                            if (ReadByte != -1)
                                client.buffer.Add((byte)ReadByte);
                        }
                        
                        if (client.buffer.Count > 0)
                        {
                            Out.WriteLine("succes");
                            foreach(ClientInfo otherClient in clients)
                            {
                                byte[] msg = client.buffer.ToArray();

                                incomingMessage = Encoding.ASCII.GetString(msg.ToArray());
                                Out.WriteLine(incomingMessage);
                                otherClient.Client.GetStream().Write(encodeMsg(incomingMessage), 0, encodeMsg(incomingMessage).Length);
                                client.buffer.Clear();
                            }
                        }
                    }
                }
                clients.RemoveAll(delegate (ClientInfo CI)
                {
                    if (!CI.isConnect)
                    {
                        Server.Out.WriteLine("disconnect");
                        return true;
                    }
                    return false;
                });
                if (newClients.Count > 0)
                {
                    clients.AddRange(newClients);
                    newClients.Clear();
                }


            }
            
        }

        ~Server()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
            foreach(ClientInfo client in clients)
            {
                client.Client.Close();
            }
        }

        static void ListenerClients()
        {
            while (true)
            {
                server.newClients.Add(new ClientInfo(server.Listener.AcceptTcpClient()));
                Out.WriteLine("New connected");
            }
        }

        public byte[] encodeMsg(string str)
        {
            string[] comands = str.Split(' ');
            int[] com = new int[comands.Length];
            try
            {
            for (int i = 0; i < comands.Length; i++)
            {
                com[i] = Convert.ToInt32(comands[i]);
            }
            
            
                if (com[0] == 0)
                {
                    outMessage += "Air Conditioner mode:";
                    if (com[1] == 1)
                    {
                        outMessage += " On, Temperature: " + com[2];
                        if (com[3] == 0)
                        {
                            outMessage += " Select: Swing";
                        }

                        if (com[3] == 1)
                        {
                            outMessage += " Select: Air flow";
                        }

                        if (com[3] == 2)
                        {
                            outMessage += " Select: Air direction";
                        }
                    }
                    else
                    {
                        outMessage += " Off";
                    }


                }

                else if (com[0] == 1)
                {
                    outMessage += "Oven mode: ";
                    if (com[1] == 1)
                    {
                        outMessage += "On, Temperature: " + com[2];
                        if (com[3] == 0)
                            outMessage += " Select : Below and above";
                        if (com[3] == 1)
                            outMessage += " Select : Only above";
                        if (com[3] == 2)
                            outMessage += " Select : Convection";
                        if (com[3] == 3)
                            outMessage += " Select : Grill";
                    }
                    else
                        outMessage += " Off";

                }

                else if (com[0] == 2)
                {
                    outMessage += "Lights: \n";
                    if (com[1] == 1)
                        outMessage += "Living room: On\n";
                    else
                        outMessage += "Living room: Off\n";
                    if (com[2] == 1)
                        outMessage += "Kitchen: On\n";
                    else
                        outMessage += "Kitchen: Off\n";
                    if (com[3] == 1)
                        outMessage += "Bathroom: On\n";
                    else
                        outMessage += "Bathroom: Off\n";
                }

                else
                {
                    outMessage += "Tepot: ";
                    if (com[1] == 1)
                        outMessage += "mode: On";
                    else
                        outMessage += "mode: Off";
                }
            }

            catch
            {
                outMessage = "Input error, try again.....";
            }
            byte[] msg = Encoding.ASCII.GetBytes(outMessage);
            
            //Console.WriteLine(outMessage);
            outMessage = "";

            return msg;
        }
        
    }


    public class ClientInfo
    {
        public TcpClient Client;
        public List<byte> buffer = new List<byte>();
        public bool isConnect;
        public ClientInfo(TcpClient Client)
        {
            this.Client = Client;
            isConnect = true;
        }
    }

    
}
