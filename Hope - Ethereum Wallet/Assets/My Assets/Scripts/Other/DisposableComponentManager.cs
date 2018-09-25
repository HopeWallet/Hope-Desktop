using System;
using System.Collections.Generic;

/// <summary>
/// Class which manages components which need to be disposed after the wallet is closed.
/// </summary>
public sealed class DisposableComponentManager : IDisposable
{
    private readonly List<IDisposable> disposableComponents = new List<IDisposable>();

    /// <summary>
    /// Adds a disposable component to the wallet.
    /// </summary>
    /// <param name="disposableComponent"> The disposable component to add to the manager. </param>
    public void AddDisposable(IDisposable disposableComponent) => disposableComponents.Add(disposableComponent);

    /// <summary>
    /// Calls dispose on all disposable components.
    /// </summary>
    public void Dispose() => disposableComponents.ForEach(disposable => disposable.Dispose());
}