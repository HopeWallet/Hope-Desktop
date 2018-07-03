using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MainThreadExecutor : IUpdater
{

    private static readonly Queue<Action> actionsToExecute = new Queue<Action>();

    public MainThreadExecutor(UpdateManager updateManager)
    {
        updateManager.AddUpdater(this);
    }

    public void UpdaterUpdate()
    {
        while (actionsToExecute.Count != 0)
            actionsToExecute.Dequeue()?.Invoke();
    }

    public static void QueueAction(Action action)
    {
        actionsToExecute.Enqueue(action);
    }

}