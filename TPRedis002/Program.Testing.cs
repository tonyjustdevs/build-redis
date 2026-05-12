using System.Text;

namespace TPRedis002;

partial class Program()
{
    public static void TestGetEncodedBytes()
    {
        //string text = "naïve ";           // contains special chars
        //string s = "";
        //Console.WriteLine(s);
        //byte[] bad = Encoding.ASCII.GetBytes(text);   // loses data!
        //// searches ascii[0-127] returns [decimal_val]
        //byte[] good = Encoding.UTF8.GetBytes(text);    // correct

        //Write($"\nbad [ASCII.GetBytes('naïve')]: ");
        //foreach (var b in bad) Write($"{b} ");
        //Write($"\ngood [UTF8.GetBytes('naïve')]:  ");
        //foreach (var b in good) Write($"{b} ");

        //var bad_str = Encoding.ASCII.GetString(bad);
        //var good_str = Encoding.UTF8.GetString(good);
        //WriteLine(bad_str);
        //WriteLine(good_str);
        string s = "🫠";

        foreach (byte b in Encoding.UTF8.GetBytes(s))
        {
            Console.Write($"{b:X2} ");
        }
    }
    

}
