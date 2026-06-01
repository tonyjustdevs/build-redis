using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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

    public static void PrintReqInUTF8Old(int b_int, byte[] b_arr, bool run = true, bool neat = false)
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

    public static void PrintHexBytesOld(int b_int, byte[] buffer, bool run = true)
    {
        if (run)
        {   //2A320D0A24340D0A
            string hex_str = Convert.ToHexString(buffer, 0, b_int);
            WriteLine("\n[e{0}][a{2}]:{1}", b_int, hex_str, hex_str.Length / 2);
        }
    }

    public static bool isEchoCmdPreArgsValidHexBytes(int b_int, byte[] buffer)
    {
        string echo_cmd_hex = "2A320D0A24340D0A4543484F0D0A24"; // == *2\r\n$4\r\nECHO\r\n$
        //*2\r\n$4\r\nECHO\r\n$3\r\naaa\r\n
        string buffer_hex = Convert.ToHexString(buffer);

        if (echo_cmd_hex == buffer_hex[..echo_cmd_hex.Length])
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

        //
        //response_payload_bytes
        string decimal_bytes_str = string.Join("", response_payload_bytes);
        WriteLine($"payload_decimal_str: {decimal_bytes_str} (exp: 979797");
        WriteLine($"payload_utf8_str: {Encoding.UTF8.GetString(response_payload_bytes)} (exp: aaa");

        // doing my best miss dangles :)

        return response_payload_bytes; // the [3] in [*2\r\n$4\r\nECHO\r\n$3\r\naaa\r\n]
    }
    public static byte[] Get_Payload_Bytes(int b_int, byte[] buffer)
    {   //*2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n        +PING\r\n
        //string fb = Encoding.UTF8.GetString(buffer[0..1]);
        //WriteLine($"first-byte: {fb}");
        return [];
    }


    public static void HandleClientOld(Socket client)
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            int b_int = client.Receive(buffer);// read buffer
            if (b_int == 0) break;//client disconnects

            #region print_debug_info
            PrintReqInUTF8(b_int, buffer); // debug info
            PrintHexBytes(b_int, buffer);
            #endregion

            string? cmd_str;
            if (!IsItValidRespCmd(buffer, b_int, out cmd_str))
            {
                client.Send(Encoding.UTF8.GetBytes("+INVALID\r\n"));
            }
            client.Send(Encoding.UTF8.GetBytes("+VALID\r\n"));
            //ProcessValidCommand(cmd_str);
            
            //if (IsItValidPingRespCmd(buffer, b_int)) 
            //{
            //    client.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));
            //}
            //else if (IsItValidEchoRespCmd(buffer, b_int, out string payload_str))
            //{
            //    //client.Send(Encoding.UTF8.GetBytes($"+{payload_str}\r\n"));
            //    client.Send(Encoding.UTF8.GetBytes($"${payload_str.Length}\r\n{payload_str}\r\n"));
            //    //$5\r\nhello\r\n
            //}
            //else
            //{
            //    client.Send(Encoding.UTF8.GetBytes("+BADCMDENDING\r\n"));

            //}








            #region payload_v1
            //byte[] payload_bytes = Get_EchoCmdPayload_Bytes(b_int, buffer);
            //var final_response_bytes = ApplyRedisProtocolToOneBulkString(payload_bytes);
            //byte[] response_payload = ParseRESPBytes(b_int, buffer);
            //byte[] response_payload = OLD_GetHexBytesFromRawBytes(b_int, buffer);
            #endregion





        }
    }

    public static void Parse_PING_Raw_Bytes(byte[] buffer, int b_int) 
    {
        //1.  receive raw_bytes convert to hexy_bytes
        string hexy_bytes = Convert.ToHexString(buffer,0,b_int);
        if (hexy_bytes == "2A310D0A24340D0A50494E470D0A")
        {
            WriteLine("received:\n{0}\n2A310D0A24340D0A50494E470D0A (expected)", hexy_bytes);
        }
        //2a. get first 16 bytes + 2b. test equals 2A310D0A24340D0A 
        if (hexy_bytes[..16] == "2A310D0A24340D0A")
        {
            WriteLine("1st-16-bytes:\n{0}\n2A310D0A24340D0A", hexy_bytes[..16]);
        }
        //3.Split `hexy_bytes` by limiter: `0D0A` to:
        //- `byte[] splited_hexy_bytes`

        string[] hexy_splits = hexy_bytes.Split("0D0A",3);
        string hexy_cmd = hexy_splits[2];
        Console.WriteLine($"hexy_cmd: {hexy_cmd} (all bytes)");
        Console.WriteLine($"hexy_cmd: {hexy_cmd[..8]} (4 bytes or 8 hex-chrs)");
        byte[] cmd_bytes = Convert.FromHexString(hexy_cmd[..8]);
        string cmd_str = Encoding.UTF8.GetString(cmd_bytes);
        Console.WriteLine($"cmd_str: {cmd_str}");
        Console.WriteLine($"cmd_sup: {cmd_str.ToUpper()}");

        if (cmd_str.ToUpper()=="PING" & hexy_bytes[..16] == "2A310D0A24340D0A")
        {
            WriteLine("its truly a piiinggggg!!");
        }
    }

    public static void Parse_PING_Raw_Bytes2(byte[] buffer, int b_int)
    {
        //1.  receive raw_bytes convert to hexy_bytes
        string hexy_bytes = Convert.ToHexString(buffer, 0, b_int);


        // STAGE-1: CHECK IF HEXY BYTES REPRESENT:
        // - 1 BYTE-STRING WITH 4 BYTES, IF SO PROCEED
        // - WHY?
        //   - because could be ping, PING, PiNg, PIng, piNG
        //   - need to convert all these version to Upper() (or lower) to match "PING" (or ping)
        
        // STAGE-2: CHECK IF COMMAND matches after Upper()
        // - COMMAND.UPPER() == "PING"

        // STAGE-1
        if (hexy_bytes[..16] == "2A310D0A24340D0A")
        {   //confirm its a *1-bytestring with $4-bytes
            WriteLine("Confirming 'command' received is a 1-byte-string of 4-bytes:\n{0}\n2A310D0A24340D0A", hexy_bytes[..16]);
        }
        else
        {
            WriteLine("received definitely not a PING command...ending...");
        }

            //3.Split `hexy_bytes` by limiter: `0D0A` to:
        string[] hexy_splits = hexy_bytes.Split("0D0A", 3);
        string hexy_cmd = hexy_splits[2];
        //   2A31 0D0A 2434 0D0A    whatreverishere_hopefully_a_respcmd0D0A
        //0: 2A31     | 1: 2434 |2: whatreverishere_hopefully_a_respcmd0D0A
        byte[] cmd_bytes = Convert.FromHexString(hexy_cmd[..8]);
        string cmd_str = Encoding.UTF8.GetString(cmd_bytes);
        if (cmd_str.ToUpper() == "PING" & hexy_bytes[..16] == "2A310D0A24340D0A")
        {
            WriteLine("its truly a piiinggggg!!");
        }
        else
        {
            WriteLine("not a ping!!");
            Console.WriteLine($"cmd_str: {cmd_str}");
            Console.WriteLine($"cmd_sup: {cmd_str.ToUpper()}");
        }
    }


    public static bool IsItValidPingRespCmd(byte[] buffer, int b_int)
    {
        //1.  receive raw_bytes convert to hexy_bytes
        WriteLine("\nentered IsItValidPingRespCmd()");

        string hexy_bytes = Convert.ToHexString(buffer, 0, b_int);

        // STAGE-1: CHECK IF HEXY BYTES REPRESENT:
        // STAGE-2: CHECK IF COMMAND matches after Upper(): COMMAND.UPPER() == "PING"

        // STAGE-1                * 1\r\n $ 4\r\n PING\r\n
        if (hexy_bytes[..16] == "2A310D0A24340D0A") // *1\r\n$4\r\nPING\r\n
        {   //confirm its a *1-bytestring with $4-bytes
            WriteLine("Confirming 'command' received is a 1-byte-string of 4-bytes:\n{0}\n2A310D0A24340D0A", hexy_bytes[..16]);
        }
        else
        {   //2-3 -> ELEMENTS
            //10-011
            //Encoding.UTF8.getstr
            //var received_elements = Encoding.UTF8.GetString(hexy_bytes[2..4]);
            var rec_elmts_str = Encoding.UTF8.GetString(Convert.FromHexString(hexy_bytes[2..4]));
            var cmd1_str = Encoding.UTF8.GetString(Convert.FromHexString(hexy_bytes[10..12]));
            //var received_elements = Encoding.UTF8.GetString(hexy_bytes);
            //var cm1_bytes = Encoding.UTF8.GetString(hexy_bytes[10..12]);


            WriteLine($"MATE YOU ARE CHOPPED: '{rec_elmts_str}' elements & cm1_bytes: '{cmd1_str}'...ending...");
            return false;
        }

        //3.Split `hexy_bytes` by limiter: `0D0A` to:
        //string[] hexy_splits = hexy_bytes.Split("0D0A", 3);
        string[] hexy_splits = hexy_bytes.Split("0D0A", 4);

        // ---------------------------------------------------
        // THREE SPLIT TEST: Expecting hexy_splits to be into 3
        //    * 1 \r\n $ 4  \r\n  P I N G
        //   2A31 0D0A 2434 0D0A 50494E470D0A
        // That is:
        // ["2A31","2434", "50494E470D0A"]
        // ---------------------------------------------------

        // ---------------------------------------------------
        // 4 SPLIT-TEST: Expecting hexy_splits to be into 3

        foreach (var hexy_split in hexy_splits)
        {   // 2A31 0D0A 2434 0D0A 50494E47 0D0A
            WriteLine($"NOW: {hexy_split}"); // 2A31 0D0A 2434 0D0A 50494E470 D0A
            //  NOW: 2A31
            //  NOW: 2434
            //  NOW: 50494E47
            //  NOW:
        }


        string hexy_ping_cmd = hexy_splits[2]; // 50494E47 AKA PING
        //    * 1 \r\n $ 4  \r\n
        //   2A31 0D0A 2434 0D0A    whatreverishere_hopefully_a_respcmd0D0A
        //0: 2A31     | 1: 2434 |2: whatreverishere_hopefully_a_respcmd0D0A
        //byte[] cmd_bytes = Convert.FromHexString(hexy_cmd[..8]); // THREE-SPLIT CODE
        byte[] cmd_ping_bytes = Convert.FromHexString(hexy_ping_cmd); // 4-SPLIT CODE


        string cmd_str = Encoding.UTF8.GetString(cmd_ping_bytes);// bytes -> str -> "ping"
        WriteLine($"cmd_str : {cmd_str} hex:{Convert.ToHexString(cmd_ping_bytes)}");
        if (cmd_str.ToUpper() == "PING")
        {
            WriteLine("its truly a piiinggggg!!");
            return true;
        }
        else
        {
            WriteLine($"NOT A PING: '{cmd_str}'\nPROCEED TO ECHOOOO...!!");
            //Console.WriteLine($"cmd_str: {cmd_str}");
            //Console.WriteLine($"cmd_sup: {cmd_str.ToUpper()}");
            return false;
        }

        //        [e14][a20]: *1\r\n$4\r\nPING\r\n

        //[e14][a14]:2A310D0A24340D0A50494E470D0A

        //entered IsItValidPingRespCmd()
        //Confirming 'command' received is a 1 - byte - string of 4 - bytes:
        //2A310D0A24340D0A
        //2A310D0A24340D0A
        //NOW: 2A31
        //NOW: 2434
        //NOW: 50494E47
        //NOW:
        //    cmd_str: PING hex:50494E47
        //its truly a piiinggggg!!
    }



    public static bool IsItValidRespCmdOld(byte[] buffer, int b_int, out string? valid_cmd_str)
    {
        WriteLine("\nentered IsItValidRespCmd()");

        string hexy_bytes = Convert.ToHexString(buffer, 0, b_int);

        string cmd_str = Encoding.UTF8.GetString(Convert
            .FromHexString(hexy_bytes.Split("0D0A", 4)[2]))
            .ToUpper();

        string arg_str = Encoding.UTF8.GetString(Convert
        .FromHexString(hexy_bytes.Split("0D0A", 4)[2]))
        .ToUpper();
        WriteLine($"cmd_str: {cmd_str}");
        bool valid_cmd_bool = false;
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



    public static bool IsItValidEchoRespCmd(byte[] buffer, int b_int, out string payload_str)
    {
        WriteLine("entered IsItValidEchoRespCmd()");
        //1.  receive raw_bytes convert to hexy_bytes
        string hexy_bytes = Convert.ToHexString(buffer, 0, b_int);

        if (hexy_bytes[..16] == "2A310D0A24340D0A")
        {   //confirm its a *1-element  $4-bytes
            WriteLine("Confirming 'command' received is a 1-element-arr-1st-element-4-bytes:\n{0}\n2A310D0A24340D0A", hexy_bytes[..16]);
        }
        else if (hexy_bytes[..16] == "2A320D0A24340D0A")
        {   //confirm its a *2-element with $4-bytes
            WriteLine("Confirming 'command' received is a 2-element-arr-1st-element-4-bytes:\n{0}\n2A320D0A24340D0A", hexy_bytes[..16]);
            //2A320D0A24340D0A6563686F0D0A24350D0A65656565650D0A    
            //2A 32 0D0A 24 34 0D0A 65 63 68 6F 0D0A 24 35 0D0A 65 65 65 65 65 0D0A
            // *  2 \r\n  $  4 \r\n  e  c  h  o \r\n  $  5 \r\n  e  e  e  e  e \r\n"
        }
        else
        {
            WriteLine("received something weirddd...ending...");
            payload_str = "failed";

            return false;
        }

        string[] hexy_splits = hexy_bytes.Split("0D0A", 5);
        //foreach (var item in hexy_splits)
        //{
        //    WriteLine(item);
        //}
        string hexy_cmd = hexy_splits[2];  

        byte[] cmd_bytes = Convert.FromHexString(hexy_cmd[..8]);
        string cmd_str = Encoding.UTF8.GetString(cmd_bytes);
        WriteLine($"cmd_str: {cmd_str} (exp: ECHO)");
        if (cmd_str.ToUpper() == "ECHO")
        {
            WriteLine("its truly a ECHOOO ECHOOO ECHOOOECHOO cehooocehoo!!");
            //2A320D0A24340D0A6563686F0D0A24350D0A65656565650D0A    
            //2A 32 0D0A 24 34 0D0A 65 63 68 6F 0D0A 24 35 0D0A 65 65 65 65 65 0D0A
            // *  2 \r\n  $  4 \r\n  e  c  h  o \r\n  $  5 \r\n  e  e  e  e  e \r\n"
            // *2 |  $4  | echo | $5| eeeee\r\n"
            //string payl_cmd = hexy_splits[4];
            //WriteLine($"payl_cmd: {payl_cmd}");
            //2A32       2434      6563686F         2435       6565656565    0D0A
            string hexy_payload = hexy_splits[4];

            string hexy_cmdbytesno = hexy_splits[3][2..];
            Console.WriteLine($"hexy_cmdbytesno: {hexy_cmdbytesno} (5)");
            byte[] hexy_cmdbytesno_bytes = Convert.FromHexString(hexy_cmdbytesno);
            string hexy_cmdbytesno_str = Encoding.UTF8.GetString(hexy_cmdbytesno_bytes);
            Console.WriteLine("hexy_cmdbytesno_str: {0}", hexy_cmdbytesno_str);

            int.TryParse(hexy_cmdbytesno_str, out int hexy_cmdbytesno_int);
            WriteLine($"hexy_cmdbytesno_int:{hexy_cmdbytesno_int}");
            byte[] payload_bytes = Convert.FromHexString(hexy_payload[..(hexy_cmdbytesno_int*2)]);
            //byte[] cmd_bytes = Convert.FromHexString(hexy_cmd[..8]);
            payload_str = Encoding.UTF8.GetString(payload_bytes);
            Console.WriteLine($"payload_str : {payload_str}");
            return true;
        }
        else
        {
            WriteLine("not a echo!!");
            Console.WriteLine($"cmd_str: {cmd_str}");
            Console.WriteLine($"cmd_sup: {cmd_str.ToUpper()}");
            payload_str = "failed";
            return false;
        }

    }

    public static byte[] OLD_ParseRESPBytes(int b_int, byte[] buffer) 
    {
        string hex_bytes = Convert.ToHexString(buffer,0,b_int);
        string utf8bytes = Encoding.UTF8.GetString(buffer,0,b_int);
        string raw = Encoding.UTF8.GetString(buffer,0,b_int);
        WriteLine($"recd utf8str: {utf8bytes}");
        WriteLine($"recd hexystr: {hex_bytes}");
        if (hex_bytes == "2A310D0A24340D0A50494E470D0A") // *1\r\n$4\r\nPING\r\n
        {
            Console.WriteLine("will send +PONG back");
            return Encoding.UTF8.GetBytes("+PONG");
        }
        else if (hex_bytes[..30] == "2A320D0A24340D0A4543484F0D0A24") // *2\r\n$4\r\nECHO\r\n$
        {//2A32 [0D0A] 2434 [0D0A] 4543484F [0D0A] 2433 [0D0A] 686579 [0D0A] 
         // * 2  \r\n   $ 4  \r\n   E C H O  \r\n   $ 3  \r\n   h e y  \r\n
         //arg_bytes_arr = hex_bytes[30..]
            var split_hex_bytes = hex_bytes.Split("0D0A", count: 5);
            var split_utf8bytes = utf8bytes.Split("\r\n", count: 5);// 
            var hex_string = string.Join(" ", split_hex_bytes);
            var utf8string = string.Join(" ", split_utf8bytes);
            WriteLine($"hex_string: {hex_string} (Split() '0D0A'");     //2A32 2434 4543484F 2435 6969696969
            WriteLine($"utf8string: {utf8string} (Split() '\\r\\n'");   // * 2  $ 4  E C H O  $ 5  i i i i i 

            //var s1of5_utf8_fbyte_arrycount     = split_utf8bytes[0]; // *2     == 2A32
            //var s2of5_utf8_cmd_dtype_bytecount = split_utf8bytes[1]; // $4     == 2434
            //var s3of5_utf8_cmd                 = split_utf8bytes[2]; // ECHO   == 4543484F
            //var s4of5_utf8_arg_dtype_bytecount = split_utf8bytes[3]; // iiiii  == 6969696969
            //var s5of5_utf8_arg                 = split_utf8bytes[4]; // $4     == 2434
            //WriteLine("utf8_s1: {0}", s1of5_utf8_fbyte_arrycount);
            //WriteLine("utf8_s2: {0}", s2of5_utf8_cmd_dtype_bytecount);
            //WriteLine("utf8_s3: {0}", s3of5_utf8_cmd);
            //WriteLine("utf8_s4: {0}", s4of5_utf8_arg_dtype_bytecount);
            //WriteLine("utf8_s5: {0}", s5of5_utf8_arg);

            //var s1of5_utf8_hex_bytes = split_hex_bytes[0]; // *2     == 2A32
            //var s2of5_utf8_hex_bytes = split_hex_bytes[1]; // $4     == 2434
            //var s3of5_utf8_hex_bytes = split_hex_bytes[2]; // ECHO   == 4543484F
            //var s4of5_utf8_hex_bytes = split_hex_bytes[3]; // iiiii  == 6969696969
            //var s5of5_utf8_hex_bytes = split_hex_bytes[4]; // $4     == 2434

            WriteLine("hex__s1: {0}", split_hex_bytes[0]);
            WriteLine("hex__s2: {0}", split_hex_bytes[1]);
            WriteLine("hex__s3: {0}", split_hex_bytes[2]);
            WriteLine("hex__s4: {0}", split_hex_bytes[3]);
            WriteLine("hex__s5: {0}", split_hex_bytes[4]); //hex__s5: 69 69 69 0D 0A
            WriteLine("hex_s5a: {0}[..^2] (exp:6969690D)", split_hex_bytes[..^2] ); //hex__s5: 69 69 69 0D [0A}
            WriteLine("hex_s5b: {0}[..^4] (exp:696969)", split_hex_bytes[..^4] ); //hex__s5:   69 69 69 |  [0D][0A] 
            //WriteLine("hex_s5c: {0}[..^6] (exp:6969)", s5of5_utf8_hex_bytes[..^6] ); //hex__s5: 69 69 


        }

        return [];

    }

    public static byte[] OLD_GetHexBytesFromRawBytes(int b_int, byte[] buffer)
    {
        string hexy_bytes = Convert.ToHexString(buffer, 0, b_int); // 13 bytes == 26 hex-characters
        WriteLine($"hex_bytes: {hexy_bytes}");

        string RespCmd = GetRespCmdViaHexBytes(hexy_bytes);
        //RespCmd-1:    ECHO
        //RespCmd-3:    PING
        //RespCmd-ETC:  ??? ERROR

        return [];
        //buffer
    }

    public static string GetRespCmdViaHexBytes(string hexy_bytes) 
    {
        
        string echo_resp_string_id = "2A320D0A24340D0A4543484F0D0A";
        string ping_resp_string_id = "2A310D0A24340D0A50494E470D0A"; // PING: *1\r\n$4\r\nPING\r\n
        //ping = "2A31 0D0A 2434 0D0A 50494E47 0D0A"; // PING: *1\r\n$4\r\nPING\r\n
        //         * 1 \r\n  $ 4  \r\n P I N G \r\n
        
        if (hexy_bytes == ping_resp_string_id)
        {
            WriteLine("do ping!");
        }
        //"".ToLower()
        //"".ToUpper()  
        else if (hexy_bytes[..echo_resp_string_id.Length] == "2A320D0A24340D0A4543484F0D0A")
        {   //"*2",  "$4",    "ECHO"
            WriteLine("do echo!!! echoo echooo");
            string[] hexy_splits = hexy_bytes.Split("0D0A", 5); // 2A32,2434,4543484F,2433,6565650D0A,
            WriteLine($"Splitting: {hexy_bytes}");
            
            // confirmed:
            // two args
            // - 1st arg: exactly 4-bytes AND 

            //foreach (var hex in hexy_splits)
            //{
            //    WriteLine(hex);
            //    //(exp: "2A32", "2434", "4543484F", "2435", "61616161610D0A")
            //    //(exp: " * 2", " $ 4", " E C H O", " $ 5", " a a a a a0D0A")
            
            //    //( "*2"        :   resp_1of5_cmd_arraycount
            //    //  "$4"        :   resp_2of5_bulkstr_bytecount
            //    //  "ECHO"      :   resp_3of5_bulkstr_payload
            //    //  "$5"        :   resp_4of5_cmdarg_bytecount
            //    //  "aaaaaD0A"? :   resp_5of5_cmdarg_payload
            //}
            
             
            string resp_1of5_cmd_arraycount     = hexy_splits[0];   // *2
            string resp_2of5_bulkstr_bytecount  = hexy_splits[1];   // "$4"
            string resp_3of5_bulkstr_cmdname    = hexy_splits[2];   // ECHO
            string resp_4of5_cmdarg_bytecount   = hexy_splits[3];   // $5
            string resp_5of5_cmdarg_payload     = hexy_splits[4];   // eeeee

            //  feat-23-case-insensitive-ascii-cmds     #23
            //  - case-insensitive-cmd-processing         #24
            WriteLine("cmd-recd-hex: {0}", resp_3of5_bulkstr_cmdname);
            WriteLine("cmd-recd-str: {0}", Convert.FromHexString(resp_3of5_bulkstr_cmdname));
            //WriteLine("cmd-recd-upp: {0}", Convert.FromHexString(resp_3of5_bulkstr_cmdname));


            //  - standard - ascii - cmd - processing     #25
            
        }
        return "bro";
    }

    bool IsAscii(string s)
    {
        return s.All(c => c <= 127);
    }


    
}
            // [case-1] {PING} cmd, No-Arguments
            //  *  1 \r \n  $  4 \r \n  P  I  N  G \r \n
            // 2A 31 0D 0A 24 34 0D 0A 50 49 4E 47 0D 0A
            //  1  2  3  4  5  6  7  8  9 10 11 12 13 14

            // [case-2] {ECHO} cmd, 1-Argument
            //  *  2 \r \n  $  4 \r \n  E  C  H  O \r \n $  3  \r \n  h  e  y \r \n
            // 2A 32 0D 0A 24 34 0D 0A 45 43 48 4F 0D 0A 24 33 0D 0A 68 65 79 0D 0A
            // 2A 32 0D 0A 24 34 0D 0A 45 43 48 4F 0D 0A 24
            //  0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 hex_bytes[..30]
            // 01 23 45 67 89 10 12 14 16 18 20 22 24 26 28
            //                11 13 15 17 19 21 23 25 27 29

// ECHO aaaaa: 
// *2\r\n$4\r\nECHO        \r\n   $   5 \r\n aaaaa\r\n
// 2A320D0A24340D0A4543484F0D0A   24 35 0D0A 61616161610D0A

//$ redis-cli PING # The command you implemented in the previous stages
//PONG
//$ redis-cli ECHO hey # The command you'll implement in this stage
//hey

