using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TPRedis002;

partial class Program
{
    static bool print_debugs = true;
    static void Main(string[] args)
    {
        Console.WriteLine("Starting TPRedis002-Server...");

        TcpListener server = new TcpListener(System.Net.IPAddress.Any, 6969);
        server.Start();
        while (true)
        {
            Socket client = server.AcceptSocket();
            Task.Run(() => HandleClient(client));
        }

    }

    public static void IsEchoCommand(byte[] buffer) 
    {
        // expected an array ["ECHO", "BYTE SAFE STRING?"]
        //"*\r\nECHO"
        string fb = Encoding.UTF8.GetString(buffer,0,1);
        //string fb = Encoding.UTF8.GetString(buffer,1,1);
        WriteLine($"1st-byte: {fb}[{buffer[0]}]");
        if (fb=="*")
        {
            WriteLine("Incoming array!!");
        }
        else
        {
            WriteLine("Not an array: {0}", fb);
        }

    }
    public static void IsValidEchoCommand(byte[] buffer)
    {   //"*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n"
        //* 2 \r \n $ 4 \r \n E C H O \r \n $ 3 \r \n h e y \r \n"
        //Encoding.UTF8.GetString()
        //BitConverter.
        //Convert.ToHexString(buffer)

    }
    public static void IsValidEchoCommandArguments() { }
    public static void ParseEchoCommand(){}

    public static void HandleClient(Socket client)
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            // read buffer
            int b_int = client.Receive(buffer);
            
            // debug info
            IsEchoCommand(buffer);
            PrintReqInUTF8(b_int, buffer);
            PrintHexBytes(b_int, buffer);

            //client disconnects
            if (b_int == 0) break; 
            

            // server response
            byte[] response_utf8_bytes = Encoding.UTF8.GetBytes("+PONG\r\n");
            WriteLine("Sending: {0} bytes", response_utf8_bytes.Length);
            client.Send(response_utf8_bytes);
        }

    }
}
