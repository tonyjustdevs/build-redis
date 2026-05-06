using System.Net.Sockets;
using System.Text;

namespace TonyRedis5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting TP Resdis Server...");

            TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
            server.Start();
            while (true)
            {   //client sends: "ECHO hey"
                Socket client = server.AcceptSocket();
                Task.Run(() => HandleClient(client));
            }
            Console.WriteLine("TP Redis Server Stopped...");
        }
        static void HandleClient(Socket client)
            {
                var con_res =client.Connected == true ? "client connected" : "client connection failed";
                Console.WriteLine(con_res);
                while (true)    
                {
                    byte[] buffer_received = new byte[1024];
                    int res = client.Receive(buffer_received);
                    var buffer_list = buffer_received.ToList();
                    if (res==0)
                    {
                        break;
                    }
                    var pong_bytes = Encoding.ASCII.GetBytes("+pongify\r\n");
                    client.Send(pong_bytes);
                }            
                Console.WriteLine("client disconnected");

            } 
    }
}
