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
            WriteLine("TP Redis Server Starting...");
            WriteLine($"args length: {args.Length   }");
            foreach (var arg in args)
            {
                Console.WriteLine($"arg: {arg}");
            }
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6970);
            server.Start();
            var client = server.AcceptSocket();

            WriteLine($"client.Connected: {client.Connected}");
            //WriteLine($"Received bytes: ");
            
            byte[] byte_buffer = new byte[42];
            
            while (true)
            {
                int nbr_of_bytes_rec = client.Receive(byte_buffer);
                if (nbr_of_bytes_rec==0)
                {
                    break;
                }
                //WriteLine($"\ncurrent buffer: ");
                //foreach (var item in byte_buffer)
                //{
                //    Write($"{item} ");
                //}
                //WriteLine($"Converted to string: {Encoding.ASCII.GetString(byte_buffer)}");
                client.Send(Encoding.ASCII.GetBytes("+PONG\r\n"));
                //client.Send(Encoding.ASCII.GetBytes("+PONG"));
            }
            WriteLine($"connection ended.\n");

            
            
            
            WriteLine("TP Redis Server Stopped...");
        }
    }
}
