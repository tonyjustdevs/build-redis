
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TonyRedis7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting TP-Redis7-Server");
            TcpListener server = new TcpListener(System.Net.IPAddress.Any,6969);
            server.Start();
            Socket client = server.AcceptSocket(); // client connected
            WriteLine("client connected: {0}", client);
            while (true)
            {
                byte[] b_arr = new byte[1024];
                int b_int = client.Receive(b_arr);
                if (b_int==0){break;} // client disconnected

                client.Send(Encoding.ASCII.GetBytes("+hPONGify\r\n"));
            }
            WriteLine("client disconnected: {0}", client is null ? "null" : "disconnceted");

            Console.WriteLine("Shutting TP-Redis7-Server");
        }
    }
}
