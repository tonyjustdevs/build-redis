using System.Net.Sockets;
using System.Text;

namespace TonyRedis2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
            server.Start();
            var client = server.AcceptSocket();
            var response_str = "+PONG\r\n";
            byte[] response_bytes = Encoding.ASCII.GetBytes(response_str);
            Console.WriteLine($"response_str: {response_str}");
            Console.WriteLine($"response_bytes: {response_bytes}");
            client.Send(response_bytes);

            // [1] build+run [windows_exec] via [visualstudio26]
            // [2] build+run [windows_exec] via [bash shell (.sh)]
            // [3] build+run [linux_exec] via [bash shell (.sh)]

        }
    }
}
