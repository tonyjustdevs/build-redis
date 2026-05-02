using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace TonyRedis3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WriteLine("TP Redis Server Starting...");

            foreach (var arg in args)
            {
                Console.WriteLine($"arg: {arg}");
            }
            //WriteLine($"\narg_0: {args[0]}");
            //WriteLine($"arg_1: {args[1]}");
            //WriteLine($"arg_2: {args[2]}");
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
            server.Start();
            var client = server.AcceptSocket();
            byte[] response_bytes = Encoding.ASCII.GetBytes("+PONG");
            client.Send(response_bytes);

            WriteLine("TP Redis Server Stopped...");
        }
    }
}
