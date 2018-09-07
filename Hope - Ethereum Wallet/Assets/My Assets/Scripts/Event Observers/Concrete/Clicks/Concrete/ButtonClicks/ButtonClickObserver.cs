using System;
using UnityEngine;

/// <summary>
/// Class used for observing button clicks and notifying the certain subscribers.
/// </summary>
public class ButtonClickObserver : ClickObserver<IButtonObservableBase, KeyCode>
{

    /// <summary>
    /// The function that alerts a click down.
    /// </summary>
    protected override Func<KeyCode, bool> DownClickFunc => code => Input.GetKeyDown(code);

    /// <summary>
    /// The function that alerts a click hold.
    /// </summary>
    protected override Func<KeyCode, bool> HoldClickFunc => code => Input.GetKey(code);

    /// <summary>
    /// The function that alerts a click.
    /// </summary>
    protected override Func<KeyCode, bool> UpClickFunc => code => Input.GetKeyUp(code);

    /// <summary>
    /// Initializes the ButtonObserver by starting all the observers.
    /// </summary>
    public ButtonClickObserver()
    {
        StartClickObservers<IEscapeButtonObservable>(KeyCode.Escape, (button, click) => button.EscapeButtonPressed(click));
        StartClickObservers<ITabButtonObservable>(KeyCode.Tab, (button, click) => button.TabButtonPressed(click));
        StartClickObservers<IEnterButtonObservable>(KeyCode.Return, (button, click) => button.EnterButtonPressed(click));
    }
}