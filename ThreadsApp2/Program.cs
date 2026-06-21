namespace ThreadsApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"[Main] Start [tid: {Thread.CurrentThread.ManagedThreadId}]");

            //Thread thread = new Thread(()=> ShoutBro());
            //thread.Start();

            //Task.Run(() => ShoutBro());// [1] immediately closes
            //var task = Task.Run(ShoutBroAsync);// immediately closes
            ////await task;
            //Console.WriteLine($"[Main] End [tid: {Thread.CurrentThread.ManagedThreadId}]");
        }
        public static async Task ShoutBroAsync()
        {
            Thread.Sleep(2000);
            Console.WriteLine($"[SHBR] sup bro! [tid: {Thread.CurrentThread.ManagedThreadId}]");
        }
        public static void ShoutBro()
        {
            Thread.Sleep(2000);
            Console.WriteLine($"[SHBR] sup bro! [tid: {Thread.CurrentThread.ManagedThreadId}]");
        }
        public static void ShoutName(string name)
        {
            Thread.Sleep(2000);
            Console.WriteLine($"[SHBR] sup {name}! [tid: {Thread.CurrentThread.ManagedThreadId}]");
        }

    }


}
