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
        {
            string hex_str = Convert.ToHexString(buffer, 0, b_int);
            WriteLine("\n[e{0}][a{2}]:{1}", b_int, hex_str, hex_str.Length/2);
        }
    }

}   

