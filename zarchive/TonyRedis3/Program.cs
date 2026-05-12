using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace TonyRedis3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TP Redis Server Starting...");
            Console.WriteLine($"args length: {args.Length   }");
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6379);
            server.Start();
            var client = server.AcceptSocket();

            Console.WriteLine($"client.Connected: {client.Connected}");
            
            byte[] byte_buffer = new byte[42];
            
            while (true)
            {
                int nbr_of_bytes_rec = client.Receive(byte_buffer);
                if (nbr_of_bytes_rec==0)
                {
                    break;
                }
                client.Send(Encoding.ASCII.GetBytes("+PONG\r\n"));
            }
            Console.WriteLine($"connection ended.\n");

            Console.WriteLine("TP Redis Server Stopped...");
        }
    }
}
