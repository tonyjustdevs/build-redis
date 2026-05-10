using System.Net.Sockets;
using System.Text;

namespace TonyRedis8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting TP-Redis7-Server [{0}]", Thread.CurrentThread.ManagedThreadId);
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
            server.Start();
            while (true)
            {
                Socket client = server.AcceptSocket(); // client connected
                Task.Run(() => HandleClient(client));
            }
            Console.WriteLine("Shutting TP-Redis7-Server [{0}]",Thread.CurrentThread.ManagedThreadId);
        }
    
        static void HandleClient(Socket client)
        {
            WriteLine("client connected [{0}]", Thread.CurrentThread.ManagedThreadId);
            WriteLine("processing client...[{0}]", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(10000);
            while (true)
            {
                byte[] b_arr = new byte[1024];
                int b_int = client.Receive(b_arr);
                if (b_int == 0) { break; } // client disconnected
                WriteLine("Sending reply: '+PONGify' [{0}]", Thread.CurrentThread.ManagedThreadId);
                client.Send(Encoding.ASCII.GetBytes("+hPONGify\r\n"));
            }
        }
    
    }
}
