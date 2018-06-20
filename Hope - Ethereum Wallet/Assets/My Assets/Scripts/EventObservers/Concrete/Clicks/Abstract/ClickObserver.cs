using System;
using System.Linq;
using UniRx;

public abstract class ClickObserver<T, TInputArg> : EventObserver<T>
{

    protected abstract Func<TInputArg, bool> DownClickFunc { get; }

    protected abstract Func<TInputArg, bool> HoldClickFunc { get;  }

    protected abstract Func<TInputArg, bool> UpClickFunc { get; }

    protected void StartClickObservers<TClickObserver>(TInputArg inputArg, Action<TClickObserver, ClickType> clickAction) where TClickObserver : T
    {
        Observable.EveryUpdate().Where(_ => DownClickFunc(inputArg)).Subscribe(_ => ButtonPressed<TClickObserver>(ClickType.Down, b => clickAction(b, ClickType.Down)));
        Observable.EveryUpdate().Where(_ => HoldClickFunc(inputArg)).Subscribe(_ => ButtonPressed<TClickObserver>(ClickType.Hold, b => clickAction(b, ClickType.Hold)));
        Observable.EveryUpdate().Where(_ => UpClickFunc(inputArg)).Subscribe(_ => ButtonPressed<TClickObserver>(ClickType.Up, b => clickAction(b, ClickType.Up)));
    }

    private void ButtonPressed<TClickObserver>(ClickType clickType, Action<TClickObserver> clickAction) where TClickObserver : T
    {
        observables.OfType<TClickObserver>().ToList().SafeForEach(button => clickAction(button));
    }

}