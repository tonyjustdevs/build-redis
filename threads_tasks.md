# 1. Threads and Tasks
`Threads` are like workers
`Tasks` are like jobs   

```C#
Task task = Task.Run(() =>
{
    Console.WriteLine("Working");
});
```
Above creates a `Task` object.
the **run-time** uses the `thread-pool` to execute

# 2. Delegates
## 2.1 Old-School Delegates
- Declare **delegate**
- Create **variable** with delegate as as the type
- Assign a **method** to this delegate

```CS

```