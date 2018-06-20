/// <summary>
/// Base interface for all concrete click observables to implement.
/// </summary>
public interface IClickObservableBase
{
}

/// <summary>
/// Interface to implement to receive callbacks on left click.
/// </summary>
public interface ILeftClickObservable : IClickObservableBase
{

    /// <summary>
    /// Called on mouse left click.
    /// </summary>
    void OnLeftClick(ClickType clickType);
}

/// <summary>
/// Interface to implement to receive callbacks on right click.
/// </summary>
public interface IRightClickObservable : IClickObservableBase
{

    /// <summary>
    /// Called on mouse right click.
    /// </summary>
    void OnRightClick(ClickType clickType);
}

/// <summary>
/// Interface to implement to receive callbacks on middle click.
/// </summary>
public interface IMiddleClickObservable : IClickObservableBase
{

    /// <summary>
    /// Called on mouse middle click.
    /// </summary>
    void OnMiddleClick(ClickType clickType);
}