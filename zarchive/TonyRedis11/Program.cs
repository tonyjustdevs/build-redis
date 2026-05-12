using System.Net.Sockets;
using System.Text;

namespace TonyRedis11;

internal class Program
{
    static void Main(string[] args)
    {
        WriteLine("Starting TP-RedisSvr-v11");
        TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
        server.Start();
        Socket client = server.AcceptSocket();
        byte[] b_arr = new byte[1024];
        while (true)
        {
            int b_int = client.Receive(b_arr);
            WriteLine($"[{b_int}]svr_recd: {Encoding.ASCII.GetString(b_arr,0,b_int)}");
            if (b_int == 0)
            {
                break;
            }
        }
        client.Send(Encoding.ASCII.GetBytes("+PONGify\r\n"));
    }
}
