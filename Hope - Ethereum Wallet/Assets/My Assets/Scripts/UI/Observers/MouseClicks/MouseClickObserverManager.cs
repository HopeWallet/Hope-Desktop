using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

/// <summary>
/// Class which observes mouse click events.
/// </summary>
public class MouseClickObserverManager
{

    private readonly List<IObserveLeftClick> leftClickObservers = new List<IObserveLeftClick>();
    private readonly List<IObserveRightClick> rightClickObservers = new List<IObserveRightClick>();
    private readonly List<IObserveMiddleClick> middleClickObservers = new List<IObserveMiddleClick>();

    /// <summary>
    /// Starts the observers.
    /// </summary>
    public MouseClickObserverManager()
    {
        StartLeftClickObservers();
        StartRightClickObservers();
        StartMiddleClickObservers();
    }

    /// <summary>
    /// Adds a new IObserveLeftClick to the list of left click observers.
    /// </summary>
    /// <param name="leftClickObserver"> The IObserveLeftClick to add. </param>
    public void AddLeftClickObserver(IObserveLeftClick leftClickObserver) => leftClickObservers.Add(leftClickObserver);

    /// <summary>
    /// Adds a new IObserveRightClick to the list of right click observers.
    /// </summary>
    /// <param name="rightClickObserver"> The IObserveRightClick to add. </param>
    public void AddRightClickObserver(IObserveRightClick rightClickObserver) => rightClickObservers.Add(rightClickObserver);

    /// <summary>
    /// Adds a new IObserveMiddleClick to the list of middle click observers.
    /// </summary>
    /// <param name="middleClickObserver"> The IObserveMiddleClick to add. </param>
    public void AddMiddleClickObserver(IObserveMiddleClick middleClickObserver) => middleClickObservers.Add(middleClickObserver);

    /// <summary>
    /// Removes an IObserveLeftClick from the list of left click observers.
    /// </summary>
    /// <param name="leftClickObserver"> The IObserveLeftClick to remove. </param>
    public void RemoveLeftClickObserver(IObserveLeftClick leftClickObserver) => leftClickObservers.Remove(leftClickObserver);

    /// <summary>
    /// Removes an IObserveRightClick from the list of right click observers.
    /// </summary>
    /// <param name="rightClickObserver"> The IObserveRightClick to remove. </param>
    public void RemoveRightClickObserver(IObserveRightClick rightClickObserver) => rightClickObservers.Remove(rightClickObserver);

    /// <summary>
    /// Removes an IObserveMiddleClick from the list of middle click observers.
    /// </summary>
    /// <param name="middleClickObserver"> The IObserveMiddleClick to remove. </param>
    public void RemoveMiddleClickObserver(IObserveMiddleClick middleClickObserver) => middleClickObservers.Remove(middleClickObserver);

    /// <summary>
    /// Called when a left click is found.
    /// </summary>
    /// <param name="clickType"> The type of left click found. </param>
    private void OnLeftClick(ClickType clickType) => leftClickObservers.SafeForEach(observer => observer.OnLeftClick(clickType));

    /// <summary>
    /// Called when a right click is found.
    /// </summary>
    /// <param name="clickType"> The type of right click found. </param>
    private void OnRightClick(ClickType clickType) => rightClickObservers.SafeForEach(observer => observer.OnRightClick(clickType));

    /// <summary>
    /// Called when a middle click is found.
    /// </summary>
    /// <param name="clickType"> The type of middle click found. </param>
    private void OnMiddleClick(ClickType clickType) => middleClickObservers.SafeForEach(observer => observer.OnMiddleClick(clickType));

    /// <summary>
    /// Starts the observers which look for left click events.
    /// </summary>
    private void StartLeftClickObservers()
    {
        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0)).Subscribe(_ => OnLeftClick(ClickType.Down));
        Observable.EveryUpdate().Where(_ => Input.GetMouseButton(0)).Subscribe(_ => OnLeftClick(ClickType.Hold));
        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(0)).Subscribe(_ => OnLeftClick(ClickType.Up));
    }

    /// <summary>
    /// Starts the observers which look for right click events.
    /// </summary>
    private void StartRightClickObservers()
    {
        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(1)).Subscribe(_ => OnRightClick(ClickType.Down));
        Observable.EveryUpdate().Where(_ => Input.GetMouseButton(1)).Subscribe(_ => OnRightClick(ClickType.Hold));
        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(1)).Subscribe(_ => OnRightClick(ClickType.Up));
    }

    /// <summary>
    /// Starts the observers which look for middle click events.
    /// </summary>
    private void StartMiddleClickObservers()
    {
        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(2)).Subscribe(_ => OnRightClick(ClickType.Down));
        Observable.EveryUpdate().Where(_ => Input.GetMouseButton(2)).Subscribe(_ => OnRightClick(ClickType.Hold));
        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(2)).Subscribe(_ => OnRightClick(ClickType.Up));
    }

}