using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace TPRedis002;
partial class Program
{

    public static void HandleClient(Socket client)
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            int b_int = client.Receive(buffer);// read buffer
            if (b_int == 0) break;//client disconnects

            #region Print Debug Info
            
            PrintReqInUTF8(b_int, buffer); // debug info
            PrintHexBytes(b_int, buffer);
            
            #endregion

            #region resp validation
            if (!IsItValidRespCmd(buffer, b_int, out string? cmd_str))// FAIL EARLY: if invalid cmd
            {   
                client.Send(Encoding.UTF8.GetBytes("+INVALID\r\n"));
            }
            #endregion


            byte[] return_bytes = ProcessCommandStr(buffer, b_int, cmd_str);
            //Encoding.UTF8.
            client.Send(return_bytes);

        }
    }

    #region Command Validation
    public static bool IsItValidRespCmd(byte[] buffer, int b_int, out string? valid_cmd_str)
    {
        WriteLine("\nentered IsItValidRespCmd()");

        string hexy_bytes = Convert.ToHexString(buffer, 0, b_int);

        string cmd_str = Encoding.UTF8.GetString(Convert
            .FromHexString(hexy_bytes.Split("0D0A", 4)[2]))
            .ToUpper();

        
        bool valid_cmd_bool = false;
        
        WriteLine($"cmd_str: {cmd_str}");
        switch (cmd_str)
        {
            case "ECHO":
                WriteLine("'ECHO' cmd received");
                valid_cmd_str = "ECHO";
                valid_cmd_bool = true;
                break;
            case "PING":
                WriteLine("'PING' cmd received");
                valid_cmd_str = "PING";
                valid_cmd_bool = true;
                break;
            default:
                WriteLine($"'{cmd_str}' IS NOT RECOGNISED!!!"); valid_cmd_str = "PING";
                //valid_cmd_str = null;
                break;
        }

        return valid_cmd_bool;
        //switc
        // https://github.com/tonyjustdevs/build-redis/issues/28

    }
    #endregion

    #region Process Command
    public static byte[] ProcessCommandStr(byte[] buffer, int b_int, string? cmd_str) 
    {
        if (cmd_str is null) return [];
        WriteLine("\nentered ProcessCommandStr('{0}')", cmd_str);

        byte[] return_bytes = cmd_str switch
        {
            "PING" => RunPingCmd(buffer, b_int),
            "ECHO" => RunEchoCmd(buffer, b_int),
            _ => []
        };
        
        WriteLine("\nleaving ProcessCommandStr([{0}]) + ret_hexy_bytes('{1}')", cmd_str, Convert.ToHexString(return_bytes));
        return return_bytes;
    }

    #endregion


    #region Command Definitions
    public static byte[] RunEchoCmd(byte[] buffer, int b_int) 
    {
        WriteLine("\nentered RunEchoCmd()");
        string hexy_bytes = Convert.ToHexString(buffer,0,b_int);

        #region debuginfo
        //WriteLine("hexy_bytes: {0}", hexy_bytes);
        //WriteLine("strs_bytes: {0}", UTF8Encoding.UTF8.GetString(buffer,0,b_int));
        // 2A31 0D0A 2434 0D0A 4563686F 0D0A - echo only
        //  * 1       $ 4       E c h o

        // [2A32] 0D0A [2434] 0D0A [4543484F] 0D0A [2433] 0D0A [696969] 0D0A []
        // [ * 2]      [ $ 4]      [ E C H O]      [ $ 3]      [ i i i]      []
        // 0            1           2               3           4            5

        // [2A32] 0D0A [2434] 0D0A [4543484F] 0D0A [24330D0A6969690D0A]
        // [ * 2]      [ $ 4]      [ E C H O]      [ $ 3 \r\ni i i\r\n]
        // 0            1           2               3                     4          
        //public String[] Split(separator, count);
        #endregion

        //string cmd_payload_hexy_str = hexy_bytes.Split("0D0A", 5)[3]; //24330D0A696969 ---  $3 iii
        string cmd_payload_hexy_str = hexy_bytes.Split("0D0A", 4)[3];  //$3\r\niii\r\n ---  $3 iii

        byte[] return_bytes = Convert.FromHexString(cmd_payload_hexy_str+"0D0A"); // [HEXY_PAYLOAD_STR] ---> [RAW_BYTES]
        WriteLine("\nleaving RunEchoCmd() + ret_hexy_bytes('{0}')", Convert.ToHexString(return_bytes));
        return return_bytes;
    }

    public static byte[] RunPingCmd(byte[] buffer, int b_int) 
    {
        WriteLine("\nentered RunPingCmd()");
        WriteLine("\nleaving RunPingCmd()");
        return Encoding.UTF8.GetBytes("+PONG\r\n");
    }
    #endregion


    #region Print Useful Information
    public static void PrintReqInUTF8(int b_int, byte[] b_arr, bool run = true, bool neat = false)
    {
        if (run & neat)
        {
            string utf8str = Encoding.UTF8.GetString(b_arr, 0, b_int);
            WriteLine("\n[e{0}][a{2}]:\n'{1}'", b_int, utf8str, utf8str.Length);
        }

        if (run & !neat)
        {
            string utf9str = Encoding.UTF8.GetString(b_arr, 0, b_int)
                .Replace("\r", "\\r").Replace("\n", "\\n");
            WriteLine("\n[e{0}][a{2}]: {1}", b_int, utf9str, utf9str.Length);
        }
    }

    public static void PrintHexBytes(int b_int, byte[] buffer, bool run = true)
    {
        if (run)
        {   //2A320D0A24340D0A
            string hex_str = Convert.ToHexString(buffer, 0, b_int);
            WriteLine("\n[e{0}][a{2}]:{1}", b_int, hex_str, hex_str.Length / 2);
        }
    }
    #endregion





}
