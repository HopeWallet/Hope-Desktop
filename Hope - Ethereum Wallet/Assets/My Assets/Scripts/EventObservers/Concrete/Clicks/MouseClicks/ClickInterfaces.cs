/// <summary>
/// Interface to implement to receive callbacks on left click.
/// </summary>
public interface IObserveLeftClick
{

    /// <summary>
    /// Called on mouse left click.
    /// </summary>
    void OnLeftClick(ClickType clickType);
}

/// <summary>
/// Interface to implement to receive callbacks on right click.
/// </summary>
public interface IObserveRightClick
{

    /// <summary>
    /// Called on mouse right click.
    /// </summary>
    void OnRightClick(ClickType clickType);
}

/// <summary>
/// Interface to implement to receive callbacks on middle click.
/// </summary>
public interface IObserveMiddleClick
{

    /// <summary>
    /// Called on mouse middle click.
    /// </summary>
    void OnMiddleClick(ClickType clickType);
}