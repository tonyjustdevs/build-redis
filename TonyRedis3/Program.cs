using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace TonyRedis3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WriteLine("TP Redis Server Starting...");
            WriteLine($"args length: {args.Length}");
            foreach (var arg in args)
            {
                Console.WriteLine($"arg: {arg}");
            }
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
            server.Start();
            var client = server.AcceptSocket();
            int byte_counter = 0;
            byte[] pong_response_bytes = Encoding.ASCII.GetBytes("+PONG\r\n");
            WriteLine($"client.Connected: {client.Connected}");
            while (client.Connected)
            {
                string expected_string = "*1\r\n$4\r\nPING\r\n"; // [1] add [expeced_string]

                // [2] convert to [expected_bytes]
                byte[] expected_bytes = Encoding.UTF8.GetBytes(expected_string);
                var expected_bytes_list = expected_bytes.ToList();
                int expected_bytes_lenght = expected_bytes_list.Count;
                // [42 49 13 10 36 52 13 10 80 73 78 71 13 10 42 49 13 10 36 52 13 10 80 73 78 71 13 10 42 49 13 10 36 52 13 10 80 73 78 71 13 10]

                byte[] bytes_chunk_received = new byte[256]; // lets assume 256 byte array
                // [3] build array piece by piece [received_bytes] 

                List<byte> current_bytes_list = new List<byte>();
                int nbr_of_bytes_recd = client.Receive(bytes_chunk_received);
                //[3a] convert [received_bytes] to [received_list]
                List<byte> bytes_chunk_received_list = bytes_chunk_received.ToList<byte>();

                byte_counter += nbr_of_bytes_recd; // [3c] incremenet [byte_counter] with [nbr_of_bytes_recd]
                if (byte_counter == expected_bytes_lenght)
                {       //check[expected_bytes] == [bytes_chunk_received_list]
                    if (expected_bytes_list.SequenceEqual(bytes_chunk_received_list))
                    {   // [3d] if [byte_counter] == [expected_bytes_count]:
                        client.Send(pong_response_bytes);
                        Console.WriteLine($"current: {bytes_chunk_received_list}");
                        bytes_chunk_received_list.Clear();
                        byte_counter = 0;
                        Console.WriteLine($"cleared: {bytes_chunk_received_list}");
                    }
                }

            }
            WriteLine("TP Redis Server Stopped...");
        }
    }
}
