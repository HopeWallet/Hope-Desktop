using System;
using System.Linq;
using UniRx;
using UnityEngine;

/// <summary>
/// Class which observes mouse click events.
/// </summary>
public class MouseClickObserver : EventObserver<IClickObservableBase>
{

    /// <summary>
    /// Starts the observers.
    /// </summary>
    public MouseClickObserver()
    {
        StartClickObservers<ILeftClickObservable>(0, (observable, clickType) => observable.OnLeftClick(clickType));
        StartClickObservers<IRightClickObservable>(1, (observable, clickType) => observable.OnRightClick(clickType));
        StartClickObservers<IMiddleClickObservable>(2, (observable, clickType) => observable.OnMiddleClick(clickType));
    }

    /// <summary>
    /// Starts the Observable for the different click types: down, hold, and up.
    /// </summary>
    /// <typeparam name="T"> The type of the IButtonObservableBase interface to observe. </typeparam>
    /// <param name="buttonKeyCode"> The number for each mouse button. </param>
    /// <param name="clickAction"> The action to call once the button is pressed. </param>
    private void StartClickObservers<T>(int buttonKeyCode, Action<T, ClickType> clickAction) where T : IClickObservableBase
    {
        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(buttonKeyCode))
                  .Subscribe(_ => ButtonPressed<T>(ClickType.Down, b => clickAction(b, ClickType.Down)));

        Observable.EveryUpdate().Where(_ => Input.GetMouseButton(buttonKeyCode))
                  .Subscribe(_ => ButtonPressed<T>(ClickType.Hold, b => clickAction(b, ClickType.Hold)));

        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(buttonKeyCode))
                  .Subscribe(_ => ButtonPressed<T>(ClickType.Up, b => clickAction(b, ClickType.Up)));
    }

    /// <summary>
    /// Looks through the observables and notifies each of them that match the type.
    /// </summary>
    /// <typeparam name="T"> The type of IClickObservableBase which will be notified. </typeparam>
    /// <param name="clickType"> The ClickType which was called on this observable. </param>
    /// <param name="clickAction"> The action to call on this observable. </param>
    private void ButtonPressed<T>(ClickType clickType, Action<T> clickAction) where T : IClickObservableBase
    {
        observables.OfType<T>().ToList().SafeForEach(button => clickAction(button));
    }
}