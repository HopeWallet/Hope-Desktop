
/// <summary>
/// Interface which allows for classes to listen for escape button presses.
/// </summary>
public interface IEscapeButtonObserver
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
public interface ITabButtonObserver
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
public interface IEnterButtonObserver
{
    /// <summary>
    /// Called when the enter button is pressed.
    /// </summary>
    /// <param name="clickType"> The type of click event, whether down, up, or holding. </param>
    void EnterButtonPressed(ClickType clickType);
}