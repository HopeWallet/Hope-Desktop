using System;
using System.Collections.Generic;

/// <summary>
/// Class that is used to execute Actions on the main thread.
/// Useful for when asynchronous tasks need to have some part updated on the main thread.
/// </summary>
public class MainThreadExecutor : IUpdater
{
    private static readonly Queue<Tuple<Action, string>> actionsToExecute = new Queue<Tuple<Action, string>>();

    /// <summary>
    /// Initializes the MainThreadExecutor with the UpdateManager reference.
    /// </summary>
    /// <param name="updateManager"> The active UpdateManager. </param>
    public MainThreadExecutor(UpdateManager updateManager)
    {
        updateManager.AddUpdater(this);
    }

    /// <summary>
    /// Executes all actions that are queued to execute.
    /// </summary>
    public void UpdaterUpdate()
    {
        while (actionsToExecute.Count != 0)
        {
            var element = actionsToExecute.Dequeue();

            if (element.Item1 == null)
                UnityEngine.Debug.Log("Action is null! Tag -> " + element.Item2);

            element.Item1?.Invoke();
            //element.Invoke();
        }
    }

    /// <summary>
    /// Queues an action for updating on the main thread.
    /// </summary>
    /// <param name="action"> The action to execute on the main thread. </param>
    public static void QueueAction(Action action, string executionTag)
    {
        actionsToExecute.Enqueue(new Tuple<Action, string>(action, executionTag));
    }
}