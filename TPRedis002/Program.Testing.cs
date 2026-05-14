using System.Net.Sockets;
using System.Text;

namespace TPRedis002;

partial class Program()
{
    public static void PrintRequestAllEncodings(int b_int, byte[] b_arr, bool run)
    {
        if (run)
        {
            string utf8str = Encoding.UTF8.GetString(b_arr, 0, b_int);
            string utf9str = Encoding.UTF8.GetString(b_arr, 0, b_int)
                .Replace("\r", "\\r").Replace("\n", "\\n");
            string hex_str = Convert.ToHexString(b_arr, 0, b_int);
            string hex2str = BitConverter.ToString(b_arr, 0, b_int);
            WriteLine("\n[e{0}][a{2}]:\n'{1}'", b_int, utf8str, utf8str.Length);
            WriteLine("[e{0}][a{2}]: {1}", b_int, utf9str, utf9str.Length);
            WriteLine("[e{0}][a{2}]: {1}", b_int, hex_str, hex_str.Length);
            WriteLine("[e{0}][a{2}]: {1}", b_int, hex2str, hex2str.Length);
        }
    }

    public static void PrintReqInUTF8(int b_int, byte[] b_arr, bool run=true, bool neat=false)
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
        

    public static void PrintHexBytes(int b_int, byte[] buffer, bool run=true)
    {
        if (run)
        {   //2A320D0A24340D0A
            string hex_str = Convert.ToHexString(buffer, 0, b_int);
            WriteLine("\n[e{0}][a{2}]:{1}", b_int, hex_str, hex_str.Length/2);
        }
    }

    public static bool isEchoCmdPreArgsValidHexBytes(int b_int, byte[] buffer)
    {
        string echo_cmd_hex = "2A320D0A24340D0A4543484F0D0A24"; // == *2\r\n$4\r\nECHO\r\n$
        //*2\r\n$4\r\nECHO\r\n$3\r\naaa\r\n
        string buffer_hex = Convert.ToHexString(buffer);

        if (echo_cmd_hex==buffer_hex[..echo_cmd_hex.Length])
        {
            Console.WriteLine("Valid-sh Echo-Cmd (pre-args):");
            Console.WriteLine($"- Expctd-Hex: {echo_cmd_hex}");
            Console.WriteLine($"- Actual-Hex: {buffer_hex[..echo_cmd_hex.Length]}");
            return true;
        }
        else
        {
            Console.WriteLine("Invalid Echo-Cmd (pre-args):");
            Console.WriteLine($"- Expctd-Hex: {echo_cmd_hex}");
            Console.WriteLine($"- Actual-Hex: {buffer_hex[..echo_cmd_hex.Length]}");

            return false;
        }
    }

    public static byte[] Get_EchoCmdPayload_Bytes_V2(int b_int, byte[] buffer)
    {
        string echo_cmd_hex = "2A320D0A24340D0A4543484F0D0A24"; // == *2\r\n$4\r\nECHO\r\n$
        int cmd_arg_hex_idx1 = echo_cmd_hex.Length;
        int cmd_arg_hex_idx2 = cmd_arg_hex_idx1 + 2;
        var buffer_hex = Convert.ToHexString(buffer);
        var hex_args_array = buffer_hex[cmd_arg_hex_idx1..cmd_arg_hex_idx2];
        var cmdarg_bulkstr_length_hexstr = string.Join("", hex_args_array);
        var cmdarg_len_b_arr = Convert.FromHexString(cmdarg_bulkstr_length_hexstr); // bytes 0x33 -> b'...'
        var cmdarg_len_b_str = Encoding.UTF8.GetString(cmdarg_len_b_arr);
        int.TryParse(cmdarg_len_b_str, out int payload_bytes_no);
        int cmd_arg_payload_idx1 = cmd_arg_hex_idx2 + 4; // move fwd 4-bytes (skipping [\r]\n<[d]ata>
        int cmd_arg_payload_idx2 = cmd_arg_payload_idx1 + (2 * payload_bytes_no); // bc hex are 2-char each
        string buffer_payload_hex = buffer_hex[cmd_arg_payload_idx1..cmd_arg_payload_idx2];
        byte[] response_payload_bytes = Convert.FromHexString(buffer_payload_hex);
        string decimal_bytes_str = string.Join("", response_payload_bytes);
        return response_payload_bytes; // the [3] in [*2\r\n$4\r\nECHO\r\n$3\r\naaa\r\n]
    }







    public static void HandleClient(Socket client)
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            // read buffer
            int b_int = client.Receive(buffer);

            //client disconnects
            if (b_int == 0) break;

            // debug info
            //PrintReqInUTF8(b_int, buffer);
            //PrintHexBytes(b_int, buffer);
            //isEchoCmdPreArgsValidHexBytes(b_int, buffer);
            byte[] payload_bytes = Get_EchoCmdPayload_Bytes(b_int, buffer);
            var final_response_bytes = ApplyRedisProtocolToOneBulkString(payload_bytes);

            client.Send(final_response_bytes);
        }

    }

    public static byte[] ApplyRedisProtocolToOneBulkString(byte[] raw_payload_bytes)
    //private static string ApplyRedisProtocolToOneBulkString(byte[] raw_payload_bytes)
    {
        Console.WriteLine("\nIn ApplyRedisProtocolToOneBulkString()...");
        Console.WriteLine($"raw_payload_bytes_str: {string.Join("", raw_payload_bytes)} (exp: 979797, req: $3\\r\\nhey\\r\\n)");
        Console.WriteLine($"raw_payload_bytes.Length: {raw_payload_bytes.Length} (exp: 3, req: 3)"); //3
        int payload_bytes = raw_payload_bytes.Length;
        //b0:       $       0x 24
        //b1:       3       0x 33
        //b2-3      \r\n    0x 0D0A
        //b4-6      aaa     0x 616161
        //b7-8      \r\n    0x 0D0A
        //StringBuilder resp_string = 
        //string utf8_resp_string = $"${payload_bytes}\\r\\n{string.Join("", raw_payload_bytes)}\\r\\n";
        //string utf8_resp_string = $"${payload_bytes}\r\n{string.Join("", raw_payload_bytes)}\r\n";
        var utf8_payload_str = Encoding.UTF8.GetString(raw_payload_bytes);
        string utf8_resp_string = $"${payload_bytes}\r\n{utf8_payload_str}\r\n";
        //encoding
        var utf8_resp_bytes = Encoding.UTF8.GetBytes(utf8_resp_string);
        //WriteLine("sending something like this: {0}",utf8_resp_string);
        //$3\\r\\nhey\\r\\n)
        // req_bytes: [b1b2....bn-1bn]

        //2A 32 0D 0A 24 34 0D 0A 45 43 48 4F 0D 0A 24 33 0D 0A 61 61 61 0D 0A
        // *  2 \r \n  $  4 \r \n  E  C  H  O \r \n  $  3 \r \n  a  a  a \r \n
        // 1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23

        return utf8_resp_bytes;
    }



    public static byte[] Get_EchoCmdPayload_Bytes(int b_int, byte[] buffer)
    {
        string echo_cmd_hex = "2A320D0A24340D0A4543484F0D0A24"; // == *2\r\n$4\r\nECHO\r\n$
        int cmd_arg_hex_idx1 = echo_cmd_hex.Length;
        int cmd_arg_hex_idx2 = cmd_arg_hex_idx1 + 2;


        WriteLine("beg_index: {0}, end_index:{1}", cmd_arg_hex_idx1, cmd_arg_hex_idx2);
        var buffer_hex = Convert.ToHexString(buffer);
        var hex_args_array = buffer_hex[cmd_arg_hex_idx1..cmd_arg_hex_idx2];
        //note:  ...0D0A24[33]\r\n<...data..>\r\n
        //2A320D0A24340D0A4543484F0D0A24
        //2A320D0A24340D0A4543484F0D0A24[33]
        var cmdarg_bulkstr_length_hexstr = string.Join("", hex_args_array);
        Console.WriteLine("hex_args_str: {0} (expected: 33)", cmdarg_bulkstr_length_hexstr);

        // "33" ----- "3"
        // starting reading [0x33]-bytes from cmd [cmd_arg_hex_idx2]
        //2A320D0A24340D0A4543484F0D0A2433[0]D0A[6]161610D0A
        var cmdarg_len_b_arr = Convert.FromHexString(cmdarg_bulkstr_length_hexstr); // bytes 0x33 -> b'...'
        var cmdarg_len_b_str = Encoding.UTF8.GetString(cmdarg_len_b_arr);
        int.TryParse(cmdarg_len_b_str, out int payload_bytes_no);

        Console.WriteLine($"payload_bytes_no: {payload_bytes_no} (exp: 3) from '0x{cmdarg_bulkstr_length_hexstr}'");

        int cmd_arg_payload_idx1 = cmd_arg_hex_idx2 + 4; // move fwd 4-bytes (skipping [\r]\n<[d]ata>
        WriteLine("payload_idx_val: [{0}][{1}] exp(val:0x6)", cmd_arg_payload_idx1, buffer_hex[cmd_arg_payload_idx1]);


        int cmd_arg_payload_idx2 = cmd_arg_payload_idx1 + (2 * payload_bytes_no); // bc hex are 2-char each
        string buffer_payload_hex = buffer_hex[cmd_arg_payload_idx1..cmd_arg_payload_idx2];
        //buffer_payload_hex = buffer_payload_hex + "0D0A";
        Console.WriteLine($"\npayload_hex: {buffer_payload_hex} (exp: 0x61 0x61 0x61)");
        byte[] response_payload_bytes = Convert.FromHexString(buffer_payload_hex);


        //response_payload_bytes
        string decimal_bytes_str = string.Join("", response_payload_bytes);
        WriteLine($"payload_decimal_str: {decimal_bytes_str} (exp: 979797");
        WriteLine($"payload_utf8_str: {Encoding.UTF8.GetString(response_payload_bytes)} (exp: aaa");
        return response_payload_bytes; // the [3] in [*2\r\n$4\r\nECHO\r\n$3\r\naaa\r\n]
    }

}

