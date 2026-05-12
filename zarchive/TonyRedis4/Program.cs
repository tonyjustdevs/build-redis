using System.Net.Sockets;
using System.Text;

namespace TonyRedis4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start TonyRedis4 Server...");

            //TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6379);
            server.Start();
            while (true)
            {
                var client = server.AcceptSocket(); 
                Task.Run(() => HandleClient(client)); // create a new thread for each client????
            }

            void HandleClient(Socket client)
            {
                Console.WriteLine("Client Connected...'{0}'", client);
                byte[] received_buffer = new byte[1024];
                while (true)
                {   // [1] receive request
                    int rec_nbr_of_bytes = client.Receive(received_buffer);
                    if (rec_nbr_of_bytes == 0)
                    {
                        break;
                    } // [2] send response
                    client.Send(Encoding.ASCII.GetBytes("+PONG\r\n"));
                }
                Console.WriteLine("Client Disconnected...");

            }

        }
    }
}
