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
            Thread.Sleep(5000);
            byte[] bytes_arr = new byte[1024]; //byte[] == [01010101, 01001000, 01010100, etc]
            while (true)
            {
                int total_bytes_received =client.Receive(bytes_arr);  
                
                if (total_bytes_received==0){break;} // disconnect.

                var hex_str =Convert.ToHexString(bytes_arr, 0, total_bytes_received); // CORRECT: Only convert received
                var ascii_str = Encoding.ASCII.GetString(bytes_arr,0, total_bytes_received);
                
                WriteLine($"{hex_str} [l:{hex_str.Length}](b:{total_bytes_received})");
                WriteLine($"\n{ascii_str} [l:{ascii_str.Length}](b:{total_bytes_received})");
                // convert to ascii for easy parasing
                client.Send(Encoding.ASCII.GetBytes("+PONG\r\n"));
                
            }

        }
    }
}
