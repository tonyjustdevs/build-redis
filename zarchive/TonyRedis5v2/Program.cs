using System.Net.Sockets;
using System.Text;

namespace TonyRedis5v2
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
        }
        static void HandleClient(Socket client)
            {
                var con_res =client.Connected == true ? "client connected" : "client connection failed";
                Console.WriteLine(con_res);
                while (true)    
                {
                    //byte[] buffer_received = new byte[1024];
                    byte[] buffer_received = new byte[30];
                    int res = client.Receive(buffer_received);
                    WriteLine("Bytes Decimal Received:");
                    foreach (var b in buffer_received){ Write($"{b} ");}
                    // 42 50 13 10 36 52 13 10 69 67 72 79 13 10 36 51 13 10 104 101 121 13 10
                    // var ascii_string =  Encoding.ASCII.GetString(buffer_received); // convert bytes to ascii chars

                //[received] bytes to ascii:
                //*1
                //$5
                //he

                //[received] bytes to ascii:
                ////llo
                //    Console.WriteLine("[re] bytes: \n{0}\n", System.Convert.ToHexString(buffer_received));
                ////sample: "2A320D0A24340D0A4D415445"
                //Console.WriteLine("[re] bytes: \n{0}\n", BitConverter.ToString(buffer_received));
                ////sample: "2A-32-0D-0A-24-34-0D-0A-4D-41-54-45"
                //Console.WriteLine("[rec] ascii: \n{0}\n", ascii_string);
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
