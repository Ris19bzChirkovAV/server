using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        const int PORT = 7777;
        static void Main(string[] args)
        {
            Console.WriteLine("Run Server.......");
            string host = System.Net.Dns.GetHostName();
            System.Net.IPAddress ip = System.Net.Dns.GetHostByName(host).AddressList[0];
            Console.WriteLine("Ip adress: " + ip.ToString());
            Server server = new Server(PORT, Console.Out);
            server.begin();
        }
    }
}
