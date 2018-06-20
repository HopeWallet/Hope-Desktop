using System;
using System.Linq;
using UniRx;
using UnityEngine;

/// <summary>
/// Class used for observing button clicks and notifying the certain subscribers.
/// </summary>
public class ButtonObserver : EventObserver<IButtonObserverBase>
{

    /// <summary>
    /// Initializes the ButtonObserver by starting all the observers.
    /// </summary>
    public ButtonObserver()
    {
        StartButtonObservers<IEscapeButtonObserver>(KeyCode.Escape, (button, click) => button.EscapeButtonPressed(click));
        StartButtonObservers<ITabButtonObserver>(KeyCode.Tab, (button, click) => button.TabButtonPressed(click));
        StartButtonObservers<IEnterButtonObserver>(KeyCode.Return, (button, click) => button.EnterButtonPressed(click));
    }

    private void StartButtonObservers<T>(KeyCode buttonKeyCode, Action<T, ClickType> buttonPressAction) where T : IButtonObserverBase
    {
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(buttonKeyCode))
                  .Subscribe(_ => ButtonPressed<T>(ClickType.Down, b => buttonPressAction(b, ClickType.Down)));

        Observable.EveryUpdate().Where(_ => Input.GetKey(buttonKeyCode))
                  .Subscribe(_ => ButtonPressed<T>(ClickType.Down, b => buttonPressAction(b, ClickType.Hold)));

        Observable.EveryUpdate().Where(_ => Input.GetKeyUp(buttonKeyCode))
                  .Subscribe(_ => ButtonPressed<T>(ClickType.Down, b => buttonPressAction(b, ClickType.Up)));
    }

    private void ButtonPressed<T>(ClickType clickType, Action<T> buttonPressAction) where T : IButtonObserverBase
    {
        observables.OfType<T>().ToList().SafeForEach(button => buttonPressAction(button));
    }

}