using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace KrIDE.Extensions;

public static class TaskExtensions
{
    public static void FireAndForget(this Task task)
    {
        task.ContinueWith(
            t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    Console.WriteLine($"Task failed with exception: {t.Exception}");
                }
            },
            TaskScheduler.Default
        );
    }
}
