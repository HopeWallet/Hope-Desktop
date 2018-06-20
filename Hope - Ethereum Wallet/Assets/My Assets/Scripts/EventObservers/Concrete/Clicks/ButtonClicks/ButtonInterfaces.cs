/// <summary>
/// Base interface for all classes that want to observe buttons.
/// </summary>
public interface IButtonObservableBase
{
}

/// <summary>
/// Interface which allows for classes to listen for escape button presses.
/// </summary>
public interface IEscapeButtonObservable : IButtonObservableBase
{
    /// <summary>
    /// Called when the escape button is pressed.
    /// </summary>
    /// <param name="clickType"> The type of click event, whether down, up, or holding. </param>
    void EscapeButtonPressed(ClickType clickType);
}

/// <summary>
/// Interface which allows for classes to listen for tab button presses.
/// </summary>
public interface ITabButtonObservable : IButtonObservableBase
{
    /// <summary>
    /// Called when the tab button is pressed.
    /// </summary>
    /// <param name="clickType"> The type of click event, whether down, up, or holding. </param>
    void TabButtonPressed(ClickType clickType);
}

/// <summary>
/// Interface which allows for classes to listen for enter button presses.
/// </summary>
public interface IEnterButtonObservable : IButtonObservableBase
{
    /// <summary>
    /// Called when the enter button is pressed.
    /// </summary>
    /// <param name="clickType"> The type of click event, whether down, up, or holding. </param>
    void EnterButtonPressed(ClickType clickType);
}