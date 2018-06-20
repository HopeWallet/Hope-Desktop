using System.Collections.Generic;

public abstract class EventObserver<TInterfaceBase>
{

    protected readonly List<TInterfaceBase> observables = new List<TInterfaceBase>();

    public void SubscribeObservable(TInterfaceBase observable)
    {
        if (observables.Contains(observable))
            return;

        observables.Add(observable);
        OnObservableAdded(observable);
    }

    public void UnsubscribeObservable(TInterfaceBase observable)
    {
        if (observables.Remove(observable))
            OnObservableRemoved(observable);
    }

    protected virtual void OnObservableAdded(TInterfaceBase observable) { }

    protected virtual void OnObservableRemoved(TInterfaceBase observable) { }

}