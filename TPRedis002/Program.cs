using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace TPRedis002;

partial class Program
{
    //static bool print_debugs = true;
    static void Main(string[] args)
    {
        Console.WriteLine("Starting TPRedis002-Server...");

        TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
        server.Start();
        while (true)
        {
            Socket client = server.AcceptSocket();
            Task.Run(() => HandleClient(client));
        }
    }
}
