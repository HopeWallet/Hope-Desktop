using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AsyncTaskScheduler
{

    private static readonly Queue<Func<Task>> Tasks = new Queue<Func<Task>>();

    private static int currentRunningCount;

    private const int MAX_CONCURRENT_TASKS = 5;

    public static void Schedule(Func<Task> task)
    {
        Tasks.Enqueue(task);
        TaskAddedOrCompleted();
    }

    private static void TaskAddedOrCompleted()
    {
        if (currentRunningCount < MAX_CONCURRENT_TASKS && Tasks.Count > 0)
            RunTask(Tasks.Dequeue());
    }

    private static async void RunTask(Func<Task> task)
    {
        currentRunningCount++;

        await task?.Invoke();

        currentRunningCount--;

        TaskAddedOrCompleted();
    }

}