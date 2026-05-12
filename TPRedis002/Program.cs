using System.Net.Sockets;
using System.Text;

namespace TPRedis002;

partial class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting TPRedis002-Server...");

        TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
        server.Start();
        Socket client = server.AcceptSocket();
        byte[] b_arr = new byte[1024];
        while (true)
        {
            int b_int = client.Receive(b_arr);
            if (b_int == 0) break; // conn closed
            WriteLine($"rec_{b_int}:");
            WriteLine($"'{Encoding.ASCII.GetString(b_arr, 0, b_int)}'");
        }
    }
}
