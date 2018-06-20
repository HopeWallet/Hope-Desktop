using System;
using System.Linq;
using UniRx;
using UnityEngine;

/// <summary>
/// Class used for observing button clicks and notifying the certain subscribers.
/// </summary>
public class ButtonObserver : EventObserver<IButtonObservableBase>
{

    /// <summary>
    /// Initializes the ButtonObserver by starting all the observers.
    /// </summary>
    public ButtonObserver()
    {
        StartButtonObservers<IEscapeButtonObservable>(KeyCode.Escape, (button, click) => button.EscapeButtonPressed(click));
        StartButtonObservers<ITabButtonObservable>(KeyCode.Tab, (button, click) => button.TabButtonPressed(click));
        StartButtonObservers<IEnterButtonObservable>(KeyCode.Return, (button, click) => button.EnterButtonPressed(click));
    }

    /// <summary>
    /// Starts the Observable for the different click types: down, hold, and up.
    /// </summary>
    /// <typeparam name="T"> The type of the IButtonObservableBase interface to observe. </typeparam>
    /// <param name="buttonKeyCode"> The KeyCode to look out for before notifying the observables. </param>
    /// <param name="buttonPressAction"> The action to call once the button is pressed. </param>
    private void StartButtonObservers<T>(KeyCode buttonKeyCode, Action<T, ClickType> buttonPressAction) where T : IButtonObservableBase
    {
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(buttonKeyCode))
                  .Subscribe(_ => ButtonPressed<T>(ClickType.Down, b => buttonPressAction(b, ClickType.Down)));

        Observable.EveryUpdate().Where(_ => Input.GetKey(buttonKeyCode))
                  .Subscribe(_ => ButtonPressed<T>(ClickType.Down, b => buttonPressAction(b, ClickType.Hold)));

        Observable.EveryUpdate().Where(_ => Input.GetKeyUp(buttonKeyCode))
                  .Subscribe(_ => ButtonPressed<T>(ClickType.Down, b => buttonPressAction(b, ClickType.Up)));
    }

    /// <summary>
    /// Looks through the observables and notifies each of them that match the type.
    /// </summary>
    /// <typeparam name="T"> The type of IButtonObservableBase which will be notified. </typeparam>
    /// <param name="clickType"> The ClickType which was called on this observable. </param>
    /// <param name="buttonPressAction"> The action to call on this observable. </param>
    private void ButtonPressed<T>(ClickType clickType, Action<T> buttonPressAction) where T : IButtonObservableBase
    {
        observables.OfType<T>().ToList().SafeForEach(button => buttonPressAction(button));
    }

}