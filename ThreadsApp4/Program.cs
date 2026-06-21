namespace ThreadsApp4;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"[main] start threads app4 [id:{Thread.CurrentThread.ManagedThreadId}]");
        DoHeavyCPUTask();
        Console.WriteLine($"[main] end [id:{Thread.CurrentThread.ManagedThreadId}]");
    }
    
    public static void DoHeavyCPUTask()
    {
        //int csum = 0;
        Thread.Sleep(3000);
        //for (int i = 0; i < 1_000_000_000; i++)
        //{
        //    csum += i;
        //}
        Console.WriteLine($"[cpu_task] heavy cpu task completed [id:{Thread.CurrentThread.ManagedThreadId}]");
    }
}
