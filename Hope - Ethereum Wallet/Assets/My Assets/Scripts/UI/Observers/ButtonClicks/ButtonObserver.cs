using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

/// <summary>
/// Class used for observing button clicks and notifying the certain subscribers.
/// </summary>
public class ButtonObserver
{

    private readonly List<IEscapeButtonObserver> escapeButtonObservers = new List<IEscapeButtonObserver>();
    private readonly List<ITabButtonObserver> tabButtonObservers = new List<ITabButtonObserver>();
    private readonly List<IEnterButtonObserver> enterButtonObservers = new List<IEnterButtonObserver>();

    /// <summary>
    /// Initializes the ButtonObserver by starting all the observers.
    /// </summary>
    public ButtonObserver()
    {
        StartEscapeButtonObservers();
        StartTabButtonObservers();
        StartEnterButtonObservers();
    }

    /// <summary>
    /// Adds the button observer for the escape button.
    /// </summary>
    /// <param name="escapeButtonObserver"> The escape button observer to add. </param>
    public void AddEscapeButtonObserver(IEscapeButtonObserver escapeButtonObserver) => escapeButtonObservers.Add(escapeButtonObserver);

    /// <summary>
    /// Removes the button observer for the escape button.
    /// </summary>
    /// <param name="escapeButtonObserver"> The escape button observer to remove. </param>
    public void RemoveEscapeButtonObserver(IEscapeButtonObserver escapeButtonObserver) => escapeButtonObservers.Remove(escapeButtonObserver);

    /// <summary>
    /// Adds the button observer for the tab button.
    /// </summary>
    /// <param name="tabButtonObserver"> The tab button observer to add. </param>
    public void AddTabButtonObserver(ITabButtonObserver tabButtonObserver) => tabButtonObservers.Add(tabButtonObserver);

    /// <summary>
    /// Removes the button observer for the tab button.
    /// </summary>
    /// <param name="tabButtonObserver"> The tab button observer to remove. </param>
    public void RemoveTabButtonObserver(ITabButtonObserver tabButtonObserver) => tabButtonObservers.Remove(tabButtonObserver);

    /// <summary>
    /// Adds the button observer for the enter button.
    /// </summary>
    /// <param name="enterButtonObserver"> The tab enter observer to add. </param>
    public void AddEnterButtonObserver(IEnterButtonObserver enterButtonObserver) => enterButtonObservers.Add(enterButtonObserver);

    /// <summary>
    /// Removes the button observer for the enter button.
    /// </summary>
    /// <param name="enterButtonObserver"> The enter button observer to remove. </param>
    public void RemoveEnterButtonObserver(IEnterButtonObserver enterButtonObserver) => enterButtonObservers.Remove(enterButtonObserver);

    /// <summary>
    /// Called when the escape button is pressed in any form.
    /// </summary>
    /// <param name="clickType"> The type of escape button click. </param>
    private void OnEscapePressed(ClickType clickType) => escapeButtonObservers.SafeForEach(observer => observer.EscapeButtonPressed(clickType));

    /// <summary>
    /// Called when the tab button is pressed in any form.
    /// </summary>
    /// <param name="clickType"> The type of tab button click. </param>
    private void OnTabPressed(ClickType clickType) => tabButtonObservers.SafeForEach(observer => observer.TabButtonPressed(clickType));

    /// <summary>
    /// Called when the enter button is pressed in any form.
    /// </summary>
    /// <param name="clickType"> The type of enter button click. </param>
    private void OnEnterPressed(ClickType clickType) => enterButtonObservers.SafeForEach(observer => observer.EnterButtonPressed(clickType));

    /// <summary>
    /// Starts all escape button observers.
    /// </summary>
    private void StartEscapeButtonObservers()
    {
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape)).Subscribe(_ => OnEscapePressed(ClickType.Down));
        Observable.EveryUpdate().Where(_ => Input.GetKey(KeyCode.Escape)).Subscribe(_ => OnEscapePressed(ClickType.Hold));
        Observable.EveryUpdate().Where(_ => Input.GetKeyUp(KeyCode.Escape)).Subscribe(_ => OnEscapePressed(ClickType.Up));
    }

    /// <summary>
    /// Starts all tab button observers.
    /// </summary>
    private void StartTabButtonObservers()
    {
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Tab)).Subscribe(_ => OnTabPressed(ClickType.Down));
        Observable.EveryUpdate().Where(_ => Input.GetKey(KeyCode.Tab)).Subscribe(_ => OnTabPressed(ClickType.Hold));
        Observable.EveryUpdate().Where(_ => Input.GetKeyUp(KeyCode.Tab)).Subscribe(_ => OnTabPressed(ClickType.Up));
    }

    /// <summary>
    /// Starts all enter button observers.
    /// </summary>
    private void StartEnterButtonObservers()
    {
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Return)).Subscribe(_ => OnEnterPressed(ClickType.Down));
        Observable.EveryUpdate().Where(_ => Input.GetKey(KeyCode.Return)).Subscribe(_ => OnEnterPressed(ClickType.Hold));
        Observable.EveryUpdate().Where(_ => Input.GetKeyUp(KeyCode.Return)).Subscribe(_ => OnEnterPressed(ClickType.Up));
    }

}