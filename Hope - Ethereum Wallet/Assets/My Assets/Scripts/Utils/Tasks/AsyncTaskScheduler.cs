using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Class used for scheduling tasks and ensuring that there are never too many tasks running at once to reduce potential overhead.
/// </summary>
public class AsyncTaskScheduler
{

    private static readonly Queue<Func<Task>> Tasks = new Queue<Func<Task>>();

    private static int currentRunningCount;

    private const int MAX_CONCURRENT_TASKS = 5;

    /// <summary>
    /// Schedules a task to be run asynchronously.
    /// </summary>
    /// <param name="task"> The func which returns a task to be run asynchronously. </param>
    public static void Schedule(Func<Task> task)
    {
        Tasks.Enqueue(task);
        TaskAddedOrCompleted();
    }
     
    /// <summary>
    /// Called when a task is added or completed.
    /// Runs a new task if tasks remain and we are under the concurrent task limit.
    /// </summary>
    private static void TaskAddedOrCompleted()
    {
        if (currentRunningCount < MAX_CONCURRENT_TASKS && Tasks.Count > 0)
            RunTask(Tasks.Dequeue());
    }

    /// <summary>
    /// Runs a task and increases the current running task count until the task is completed.
    /// </summary>
    /// <param name="task"> The func returning the task to execute. </param>
    private static async void RunTask(Func<Task> task)
    {
        currentRunningCount++;

        await task?.Invoke();

        currentRunningCount--;

        TaskAddedOrCompleted();
    }

}