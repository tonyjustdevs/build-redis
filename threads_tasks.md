# Threads and Tasks
`Threads` are like workers
`Tasks` are like jobs   

### Level 1: Synchronous Work (the baseline)
- What: current `thread` is actively calculating something
- Examples: doing maths, loop through list, sorting data, force pause with `Thread.Sleep()`

### Level 2: Asynchronous I/O Work (`async`/`await`)
- What: current `thread` asks ***hardware*** to do something
- Examples: download file, query db, read file from hd, use a timer
- Important: **Do Not** use `Task.Run` for **I/O** work
- Code Samples: 
```C#
await Task.Delay(2000);
await File.ReadAllTextAsync("myFile.txt")
```

### Level 3: CPU-Bound Problem
What: Heavy work on **Main Thread**
Problem: `async`/`await` does not help. 
- Why? What it does for I/O operations is while it is waiting for some response from an external source, like data


