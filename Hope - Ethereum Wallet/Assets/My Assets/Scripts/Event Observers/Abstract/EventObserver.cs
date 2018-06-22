using System;
using System.Collections.Generic;

/// <summary>
/// Base class for all observer classes to extend.
/// </summary>
/// <typeparam name="TObservableBase"> The observable interface. </typeparam>
public abstract class EventObserver<TObservableBase>
{

    protected readonly List<TObservableBase> observables = new List<TObservableBase>();

    /// <summary>
    /// The type of the observable.
    /// </summary>
    public Type ObservableType => typeof(TObservableBase);

    /// <summary>
    /// Subscribes the observable to the current list of observables.
    /// </summary>
    /// <param name="observable"> The observable to subscribe for callbacks. </param>
    public void SubscribeObservable(TObservableBase observable)
    {
        if (observables.Contains(observable))
            return;

        observables.Add(observable);
        OnObservableAdded(observable);
    }

    /// <summary>
    /// Unsubscribes the observable from the list of observables.
    /// </summary>
    /// <param name="observable"> The observable to stop receiving callbacks for. </param>
    public void UnsubscribeObservable(TObservableBase observable)
    {
        if (observables.Remove(observable))
            OnObservableRemoved(observable);
    }

    /// <summary>
    /// Called when an observable is added.
    /// </summary>
    /// <param name="observable"> The observable added to the list. </param>
    protected virtual void OnObservableAdded(TObservableBase observable) { }

    /// <summary>
    /// Called when an observable is removed.
    /// </summary>
    /// <param name="observable"> The observable removed from the list. </param>
    protected virtual void OnObservableRemoved(TObservableBase observable) { }

}