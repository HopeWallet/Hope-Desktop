using System;
using UnityEngine;

/// <summary>
/// Class which observes mouse click events.
/// </summary>
public class MouseClickObserver : ClickObserver<IClickObservableBase, int>
{

    /// <summary>
    /// The function that alerts a click down.
    /// </summary>
    protected override Func<int, bool> DownClickFunc => val => Input.GetMouseButtonDown(val);

    /// <summary>
    /// The function that alerts a click hold.
    /// </summary>
    protected override Func<int, bool> HoldClickFunc => val => Input.GetMouseButton(val);

    /// <summary>
    /// The function that alerts a click up.
    /// </summary>
    protected override Func<int, bool> UpClickFunc => val => Input.GetMouseButtonUp(val);

    /// <summary>
    /// Starts the observers.
    /// </summary>
    public MouseClickObserver()
    {
        StartClickObservers<ILeftClickObservable>(0, (observable, clickType) => observable.OnLeftClick(clickType));
        StartClickObservers<IRightClickObservable>(1, (observable, clickType) => observable.OnRightClick(clickType));
        StartClickObservers<IMiddleClickObservable>(2, (observable, clickType) => observable.OnMiddleClick(clickType));
    }
}