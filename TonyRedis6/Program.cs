using System.Dynamic;
using System.Net.Sockets;
using System.Text;

namespace TonyRedis6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting TP-Redis-Server-6!");
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
            server.Start();
            WriteLine($"main thread: [id: {System.Threading.Thread.CurrentThread.ManagedThreadId}]");

            while (true)
            {
                Socket client = server.AcceptSocket();
                Task.Run(()=> HandleClient(client));
            }
        }
        static void HandleClient(Socket client)
        {
            WriteLine($"client connected: [id: {System.Threading.Thread.CurrentThread.ManagedThreadId}]");
            //Thread.Sleep(5000);
            byte[] b_arr = new byte[1024]; //byte[] == [01010101, 01001000, 01010100, etc]
            while (true)
            {
                int b_int =client.Receive(b_arr);  
                
                if (b_int==0){break;} // disconnect.

                var hex_str =Convert.ToHexString(b_arr, 0, b_int); 
                var ascii_str = Encoding.ASCII.GetString(b_arr,0, b_int);
                
                WriteLine($"hex: {hex_str}");
                WriteLine($"asc: {ascii_str}");
                
                client.Send(Encoding.ASCII.GetBytes("+PONG\r\n"));
                //client.Send("+PONG\r\n");

            }

        }
    }
}
