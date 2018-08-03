/// <summary>
/// Class which is a button that contains info at the same time.
/// </summary>
/// <typeparam name="TButton"> The type which is of ButtonBase. </typeparam>
/// <typeparam name="TValue"> The value this button should hold. </typeparam>
public abstract class InfoButton<TButton, TValue> : FactoryButton<TButton> where TButton : ButtonBase
{
    /// <summary>
    /// The info of this button.
    /// </summary>
    public TValue ButtonInfo { get; private set; }

    /// <summary>
    /// Sets the button info to the input value.
    /// </summary>
    /// <param name="info"> The value to set the button info. </param>
    /// <returns> Returns the button. </returns>
    public TButton SetButtonInfo(TValue info)
    {
        ButtonInfo = info;
        OnValueUpdated(info);
        return this as TButton;
    }

    /// <summary>
    /// Called internally if the button needs to be notified of a new value being set.
    /// </summary>
    /// <param name="info"> The new value to use in the assignment. </param>
    protected virtual void OnValueUpdated(TValue info) { }
}