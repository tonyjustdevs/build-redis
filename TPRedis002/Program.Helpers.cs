using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace TPRedis002;
partial class Program
{
    static ConcurrentDictionary<string, string> resp_keys_global_dict = [];
    public static void HandleClient(Socket client)
    {
        Console.WriteLine($"\nClient connected: {client.Connected} [id:{Thread.CurrentThread.ManagedThreadId}]");
        //Thread.Sleep(5000);
        //Dictionary<string, string> resp_keys_global_dict = new Dictionary<string, string>();


        byte[] buffer = new byte[1024];

        while (true)
        {
            int b_int = client.Receive(buffer);// read buffer
            //if (b_int == 0) {
            //    WriteLine($"client disconnects: (b_int==0) ");
            //    break;
            //}

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

            WriteLine($"gdict.count: {resp_keys_global_dict.Count}");
            client.Send(return_bytes);

        }
    }

    #region Command Validation

    public static bool IsItValidRespCmd(byte[] buffer, int b_int, out string? valid_cmd_str)
    {
        WriteLine("- entered IsItValidRespCmd()");

        string hexy_bytes = Convert.ToHexString(buffer, 0, b_int);

        string cmd_str = Encoding.UTF8.GetString(Convert
            .FromHexString(hexy_bytes.Split("0D0A", 4)[2]))
            .ToUpper();

        
        bool valid_cmd_bool = true;
        
        WriteLine($"- cmd_str: {cmd_str}");
        switch (cmd_str)
        {
            case "ECHO":
                WriteLine("- 'ECHO' cmd received");
                valid_cmd_str = "ECHO";
                break;
            case "PING":
                WriteLine("- 'PING' cmd received");
                valid_cmd_str = "PING";
                break;
            case "SET":
                WriteLine("- 'SET' cmd received");
                valid_cmd_str = "SET";
                break;
            case "GET":
                WriteLine("- 'GET' cmd received");
                valid_cmd_str = "GET";
                break;

            default:
                WriteLine($"- '{cmd_str}' IS NOT RECOGNISED!!!");
                valid_cmd_bool = false;
                valid_cmd_str = null;
                break;
        }

        return valid_cmd_bool; // https://github.com/tonyjustdevs/build-redis/issues/28

    }
    #endregion

    #region Process Command
    public static byte[] ProcessCommandStr(byte[] buffer, int b_int, string? cmd_str) 
    {
        if (cmd_str is null) return [];
        WriteLine("- entered ProcessCommandStr('{0}')", cmd_str);

        byte[] return_bytes = cmd_str switch
        {
            "PING"  => RunPingCmd(buffer, b_int),
            "ECHO"  => RunEchoCmd(buffer, b_int),
            "SET"   => RunSetCmd(buffer, b_int),
            "GET"   => RunGetCmd(buffer, b_int),
            _ => []
        };      
        
        WriteLine("- leaving ProcessCommandStr([{0}]) + ret_hexy_bytes('{1}')", cmd_str, Convert.ToHexString(return_bytes));
        return return_bytes;
    }

    #endregion

    #region Command Argument Validation 
    public static bool isEchoArgValid(string hexy_bytes_4_bytes)
    {
        Console.WriteLine($"- First 4 bytes of 'ECHO': {hexy_bytes_4_bytes}");
        if (hexy_bytes_4_bytes != "2A32") 
        {
        Console.WriteLine($"- [BAD] 'ECHO' ARGS");
            return false;
        } 
        Console.WriteLine($"- [OK] 'ECHO' ARGS");
        return true;
        

        //WriteLine("hexy_bytes: {0}", hexy_bytes);
        //WriteLine("strs_bytes: {0}", UTF8Encoding.UTF8.GetString(buffer,0,b_int));
        // 2A31 0D0A 2434 0D0A 4563686F 0D0A - echo only
        //  * 1       $ 4       E c h o

            // [2A32] 0D0A [2434] 0D0A [4543484F] 0D0A [24330D0A6969690D0A]
            // [ * 2]      [ $ 4]      [ E C H O]      [ $ 3 \r\ni i i\r\n]
            // 0            1           2               3                     4       

            //hexy_bytes == "2A310D0A.." or "2A320D0A.."
    }

    public static bool isSetArgValid(string hexy_bytes_4_bytes)
    {
        Console.WriteLine($"- First 4 bytes of 'SET': {hexy_bytes_4_bytes} (*3)");
        //if (hexy_bytes_4_bytes == "2A32")
        if (hexy_bytes_4_bytes == "2A33") // 33 - 3, 32 - 2, 31 - 1, 30 - 1
        {
            Console.WriteLine($"- [OK] 3-ELEMENT-ARRAY: 1 CMD + 2 ARG (EXP: 3 EL-ARR");
            return true;
        }
        else if (hexy_bytes_4_bytes == "2A32")
        {
            Console.WriteLine($"- [BAD] 2-ELEMENT-ARRAY: 1 CMD + 1 ARG (EXP: 3 EL-ARR");
            return false;
        }
        else
        {
            Console.WriteLine($"- [BAD] NON-3-ELEMENT-ARRAY: (EXP: 3 EL-ARR");
            return false;
        }
    }

    public static bool isGetArgValid(string hexy_bytes_4_bytes)
    {   // *2\r\n$3\r\nGET\r\n$3\r\ncat\r\n 
        Console.WriteLine($"- [isGetArgValid()] First 4 bytes of 'GET': {hexy_bytes_4_bytes} (*3)");
        //if (hexy_bytes_4_bytes == "2A32")
        if (hexy_bytes_4_bytes == "2A32") // 33 - 3, 32 - 2, 31 - 1, 30 - 1
        {   //2A == * , 32 = 2 AKA 2-EL-ARR BC [GET] + [KEY]
            Console.WriteLine($"- [isGetArgValid()] [OK] 2-ELEMENT-ARRAY: 1 CMD + 1 ARG (EXP: 2 EL-ARR");
            return true;
        }
        //else if (hexy_bytes_4_bytes == "2A32")
        //{
        //    Console.WriteLine($"- [BAD] 2-ELEMENT-ARRAY: 1 CMD + 1 ARG (EXP: 3 EL-ARR");
        //    return false;
        //}
        else
        {
            Console.WriteLine($"- [isGetArgValid()] [BAD] NON-3-ELEMENT-ARRAY: (EXP: 2 EL-ARR");
            return false;
        }
    }


    #endregion

    #region Command Definitions
    public static byte[] RunEchoCmd(byte[] buffer, int b_int) 
    {
        WriteLine("- entered RunEchoCmd()");

        string hexy_bytes = Convert.ToHexString(buffer,0,b_int);
        if (!isEchoArgValid(hexy_bytes[0..4]))
        {
            return Encoding.UTF8.GetBytes("+INVALID\r\n");
        }
        

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
        WriteLine("- leaving RunEchoCmd() + ret_hexy_bytes('{0}')", Convert.ToHexString(return_bytes));
        return return_bytes;
    }

    public static byte[] RunPingCmd(byte[] buffer, int b_int) 
    {
        WriteLine("- entered RunPingCmd()");
        WriteLine("- leaving RunPingCmd()");
        return Encoding.UTF8.GetBytes("+PONG\r\n");
    }

    public static byte[] RunSetCmd(byte[] buffer, int b_int)
    {
        WriteLine("- entered RunSetCmd()");

        string hexy_bytes = Convert.ToHexString(buffer, 0, b_int);
        if (!isSetArgValid(hexy_bytes[0..4])) // CHECKS IF IT IS 3 ELEMENT ARRAY AS EXPECTED
            {
                return Encoding.UTF8.GetBytes("+INVALID\r\n");
        }

        //- [e34][a48]:  * 3 \r\n  $ 3 \r\n  S E T \r\n  $ 3 \r\n  f o o \r\n  $ 6 \r\n  b a r b a r \r\n
        //- [e34][a34]: 2A33 0D0A 2433 0D0A 534554 0D0A 2433 0D0A 666F6F 0D0A 2436 0D0A 626172626172 0D0A

        #region debuginfo
        WriteLine("- hexy_bytes: {0}", hexy_bytes);
        WriteLine("- strs_bytes: {0}", UTF8Encoding.UTF8.GetString(buffer, 0, b_int)); // *3
        #endregion

        //string cmd_payload_hexy_str = hexy_bytes.Split("0D0A", 1)[0];  //$3\r\niii\r\n ---  $3 iii
        string[] cmd_payload_hexy_str = hexy_bytes.Split("0D0A", 8);  //$3\r\niii\r\n ---  $3 iii

        //for (int i = 0; i < cmd_payload_hexy_str.Length; i++)
        //{
        //    WriteLine($"- [{i}]: '{cmd_payload_hexy_str[i]}'") ;
        //}
        //- [0]: '2A33'     *3
        //- [1]: '2433'     $3
        //- [2]: '534554'   SET
        //- [3]: '2433'     $3      ---- len of 'key' payload: hexy[3] is the KEY == $3 is bulkstring+size: capture 3rd to end ie [3..]
        //- [4]: '666F6F'   foo     ---- acutal 'key' payload
        //- [5]: '2433'     $3      ---- len of 'val'
        //- [6]: '626172'   $bar    ---- actual 'val' payload
        //- [7]: ''

        // capture KEY

        string set_key_payload = cmd_payload_hexy_str[4]; // this is len of payload
        string set_val_payload = cmd_payload_hexy_str[6]; // this is len of payload
        
        WriteLine($"- key_payload: {set_key_payload}");
        WriteLine($"- val_payload: {set_val_payload}");

        //Dictionary<string, string> resp_keys_dict = new Dictionary<string, string>();

        //resp_keys_dict.Add(set_key_payload, "test_val"); // test val
        resp_keys_global_dict.TryAdd(set_key_payload, set_val_payload);
        WriteLine($"- resp_keys_global_dict[{set_key_payload}]: '{resp_keys_global_dict[set_key_payload]}'");

        if (resp_keys_global_dict?.ContainsKey(set_key_payload) == true)
        {
            WriteLine($"gdict[{set_key_payload}]: {resp_keys_global_dict[set_key_payload]}");
        }

        byte[] return_bytes = UTF8Encoding.UTF8.GetBytes("+OK\r\n");

        //byte[] return_bytes = Convert.FromHexString("0D0A"); // [HEXY_PAYLOAD_STR] ---> [RAW_BYTES]
        //byte[] return_bytes = Convert.FromHexString(cmd_payload_hexy_str + "0D0A"); // [HEXY_PAYLOAD_STR] ---> [RAW_BYTES]
        WriteLine("- leaving RunSetCmd() + ret_hexy_bytes('{0}')", Convert.ToHexString(return_bytes));
        //WriteLine("- leaving RunSetCmd()...");
        return return_bytes;
    }
    
    public static byte[] RunGetCmd(byte[] buffer, int b_int)
    {
        WriteLine("- [RunGetCmd()] entered RunGetCmd()");

        string hexy = System.Convert.ToHexString(buffer,0, b_int);
        string utf8 = UTF8Encoding.UTF8.GetString(buffer, 0, b_int);
        if (!isGetArgValid(hexy[0..4])) // CHECKS IF IT IS 3 ELEMENT ARRAY AS EXPECTED
        {
            return Encoding.UTF8.GetBytes("+INVALID\r\n");
        }

        // 2A32 0D0A 2433 0D0A 474554 0D0A 2433 0D0A 636174 0D0A
        //   *2 \r\n  $ 3 \r\n  G E T \r\n  $ 3  \r\n c a t \r\n 

        WriteLine($"- [RunGetCmd()] get_hexy: {hexy}"); // actual: *2\r\n$3\r\nGET\r\n$3\r\ncat
        string get_key_hexy = hexy.Split("0D0A", 6)[4];
        //string get_keylen_hexy = hexy.Split("0D0A", 6)[3]; // 2433 or $3

        WriteLine($"- [RunGetCmd()] get_key_hexy: {get_key_hexy}");
        byte[] key_bytes = System.Convert.FromHexString(get_key_hexy);

        WriteLine($"- [RunGetCmd()] get_key_utf8: {UTF8Encoding.UTF8.GetString(key_bytes)}");

        //if (resp_keys_global_dict.ContainsKey(get_key_hexy) == true)
        //{
        //    WriteLine($"gdict[636174]: {resp_keys_global_dict["636174"]}");
        //}
        //else
        //{
        //    WriteLine($"gdict count: {resp_keys_global_dict.Count()}");
            
        //}

        WriteLine($"- [RunGetCmd()] resp_keys_global_dict[{get_key_hexy}]: '{resp_keys_global_dict[get_key_hexy]}'");
        string hexy_get_val = resp_keys_global_dict[get_key_hexy];
        //while(hexy_get_val)
        //WriteLine($"- hexy_get_val: {hexy_get_val}");
        //WriteLine($"- hexy_get_val_rn: {"+"hexy_get_val+"0D0A"}");
        //byte[] return_bytes = System.Convert.FromHexString("+"+hexy_get_val+"0D0A");

        // Encoding.UTF8.GetBytes("+tempOK\r\n");
        //resp_keys_global_dict[636174]: '69696969'

        //`$3\r\nbar\r\n`
        // "$N"
        //"2A" + hex of (int)hexy_get_val
        //
        //string get_vallen_hexy;
        //string return_hex = get_vallen_hexy+"0D0A"+hexy_get_val+"0D0A"; // 2433 or $3
        //  $ 3  \r\n     bar        \r\n
        //string hexy_get_val_len= $"{hexy_get_val.Length}";


        byte[] get_val_len_utf8= UTF8Encoding.UTF8.GetBytes($"{hexy_get_val.Length}");
        string get_val_len_hexy = System.Convert.ToHexString(get_val_len_utf8);
        WriteLine($"- [RunGetCmd()] get_val_len_ints: {hexy_get_val.Length}");
        WriteLine($"- [RunGetCmd()] get_val_len_hexy: {get_val_len_hexy}");

        /// convert [hex_str] to [bytes] to [utf8], then to [bytes]
        //byte[] return_bytes =  Convert.FromHexString(return_hex);

        //byte[] return_bytes = Encoding.UTF8.GetBytes(return_hex);
        byte[] return_bytes = Encoding.UTF8.GetBytes("+TESTOK");
        return return_bytes;

    }


    #endregion

    #region Print Useful Information
    public static void PrintReqInUTF8(int b_int, byte[] b_arr, bool run = true, bool neat = false)
    {
        if (run & neat)
        {
            string utf8str = Encoding.UTF8.GetString(b_arr, 0, b_int);
            WriteLine("-  [e{0}][a{2}]:\n'{1}'", b_int, utf8str, utf8str.Length);
        }

        if (run & !neat)
        {
            string utf9str = Encoding.UTF8.GetString(b_arr, 0, b_int)
                .Replace("\r", "\\r").Replace("\n", "\\n");
            WriteLine("- [e{0}][a{2}]: {1}", b_int, utf9str, utf9str.Length);
        }
    }

    public static void PrintHexBytes(int b_int, byte[] buffer, bool run = true)
    {
        if (run)
        {   //2A320D0A24340D0A
            string hex_str = Convert.ToHexString(buffer, 0, b_int);
            WriteLine("- [e{0}][a{2}]:{1}", b_int, hex_str, hex_str.Length / 2);
        }
    }
    #endregion





}
