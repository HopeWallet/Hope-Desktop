using System;
using System.Threading.Tasks;

/// <summary>
/// Class which has several helper methods for starting common tasks.
/// </summary>
public static class TaskUtils
{

    /// <summary>
    /// Starts a string task.
    /// </summary>
    /// <param name="stringFunc"> The func to run. </param>
    /// <returns> The task which has started running. </returns>
    public static Task<string> StartStringTask(Func<string> stringFunc)
    {
        Task<string> task = new Task<string>(() => stringFunc?.Invoke());
        task.Start();

        return task;
    }

}
