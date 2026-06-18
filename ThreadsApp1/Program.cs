using System;
using System.Threading.Channels;

namespace ThreadsApp1;

//delegate void ShoutSoDelegate(string input_str);
internal class Program
{
    static void Main(string[] args)
    {
        //TaskIssues64();
        TaskIssue65();
    }
    public static void TaskIssues64()
    {

        Console.WriteLine($"[Main] Started TaskIssues64() [{Thread.CurrentThread.ManagedThreadId}]");
        Thread thread = new Thread(TPSH);
        thread.Start();
        thread.Join();
        Console.WriteLine($"[Main] Completed TaskIssues64() [{Thread.CurrentThread.ManagedThreadId}]");
    }

    public static void TaskIssue65()    
    {
        Console.WriteLine($"[main] started {Thread.CurrentThread.ManagedThreadId}");
        Thread thread = new Thread(() => Console.WriteLine($"[lamb] sup TaskIssue65! [{Thread.CurrentThread.ManagedThreadId}]"));
        thread.Start();
        thread.Join();
        Console.WriteLine($"[main] completed {Thread.CurrentThread.ManagedThreadId}");
    }


    public static void TPSH()
    {
        Console.WriteLine($"[TPSH] sup 69! [{Thread.CurrentThread.ManagedThreadId}]");
    }

}
