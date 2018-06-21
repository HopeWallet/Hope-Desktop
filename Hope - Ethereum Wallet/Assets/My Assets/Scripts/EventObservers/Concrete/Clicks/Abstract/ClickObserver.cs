using System;
using System.Linq;
using UniRx;

/// <summary>
/// Base class for other classes that have click events on certain keyboard buttons or mouse clicks for example.
/// </summary>
/// <typeparam name="T"> The type of the class inheriting this class. </typeparam>
/// <typeparam name="TInputArg"> The type of the input argument to pass through to Input.xyz when checking for a click. </typeparam>
public abstract class ClickObserver<T, TInputArg> : EventObserver<T>
{

    /// <summary>
    /// The function called on the click down action.
    /// </summary>
    protected abstract Func<TInputArg, bool> DownClickFunc { get; }

    /// <summary>
    /// The function called when the click is held.
    /// </summary>
    protected abstract Func<TInputArg, bool> HoldClickFunc { get;  }

    /// <summary>
    /// The function called when the click is released.
    /// </summary>
    protected abstract Func<TInputArg, bool> UpClickFunc { get; }

    /// <summary>
    /// Starts the click observers by observing for down, hold, and up clicks.
    /// </summary>
    /// <typeparam name="TClickObserver"> The specific type of the click observer to start the down, hold, and up click observations for. </typeparam>
    /// <param name="inputArg"> The value to pass into the Down, Hold, and Up click functions to check for a click. </param>
    /// <param name="clickAction"> The action to call once the click has been detected. </param>
    protected void StartClickObservers<TClickObserver>(TInputArg inputArg, Action<TClickObserver, ClickType> clickAction) where TClickObserver : T
    {
        Observable.EveryUpdate().Where(_ => DownClickFunc(inputArg)).Subscribe(_ => OnClick<TClickObserver>(ClickType.Down, b => clickAction(b, ClickType.Down)));
        Observable.EveryUpdate().Where(_ => HoldClickFunc(inputArg)).Subscribe(_ => OnClick<TClickObserver>(ClickType.Hold, b => clickAction(b, ClickType.Hold)));
        Observable.EveryUpdate().Where(_ => UpClickFunc(inputArg)).Subscribe(_ => OnClick<TClickObserver>(ClickType.Up, b => clickAction(b, ClickType.Up)));
    }

    /// <summary>
    /// Called when a click has been alerted.
    /// </summary>
    /// <typeparam name="TClickObserver"> The specific type of the click observer to notify all instances of. </typeparam>
    /// <param name="clickType"> The type of the click, whether Down, Hold, or Up. </param>
    /// <param name="clickAction"> The action to call on each instance of TClickObserver. </param>
    private void OnClick<TClickObserver>(ClickType clickType, Action<TClickObserver> clickAction) where TClickObserver : T
    {
        observables.OfType<TClickObserver>().ToList().SafeForEach(button => clickAction(button));
    }

}