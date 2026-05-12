using System.Net.Sockets;
using System.Text;

namespace TonyRedis8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting TP-Redis8-Server [{0}]", Thread.CurrentThread.ManagedThreadId);
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
            server.Start();
            while (true)
            {
                Socket client = server.AcceptSocket(); // client connected
                Task.Run(() => HandleClient(client));
            }
        }
    
        static void HandleClient(Socket client)
        {
            WriteLine("\n\nclient connected [{0}]", Thread.CurrentThread.ManagedThreadId);
            byte[] b_arr = new byte[1024];
            int i = 0;
            while (true)
            {
                int b_int = client.Receive(b_arr);

                if (b_int == 0) { break; }
                WriteLine($"recd_{i}: {Encoding.ASCII.GetString(b_arr)} [b_int: {b_int}]");
                //[BUG] client only sends one packet and then waits,
                //      server waits too.
                client.Send(Encoding.ASCII.GetBytes("+processing...\r\n"));
            }
            client.Send(Encoding.ASCII.GetBytes("+PONGification!\r\n"));
        }
    
    }
}
