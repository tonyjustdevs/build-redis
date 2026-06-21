using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Threading.Tasks;

namespace ThreadApp3;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("[main] ThreadsApp3");

        Console.WriteLine($"[main] pre-heavy task msg [id: {Thread.CurrentThread.ManagedThreadId}]");
        //var task = Task.Run(() => SomeSleepyTask());
        //await Task.Run(() => SomeHeavyTask()); // [PAUSES METHOD] til task completed!!
        await Task.Run(() => SomeDelayedTask()); // [PAUSES METHOD] til task completed!!

        Console.WriteLine($"[main] [post-task.run-pre.wait] msg1 [id: {Thread.CurrentThread.ManagedThreadId}]");
        
    }

    static void SomeDelayedTask()
    {
        Task.Delay(2000);
        Console.WriteLine($"[task] im sleepy [id: {Thread.CurrentThread.ManagedThreadId}]");
    }

    static void SomeHeavyTask()
    {

        BigInteger csum = 0;
        for (int i = 0; i < 100_000_000; i++)
        {
            csum += i;
        }
        Console.WriteLine($"[task] {csum} [id: {Thread.CurrentThread.ManagedThreadId}]");
    }
    }
