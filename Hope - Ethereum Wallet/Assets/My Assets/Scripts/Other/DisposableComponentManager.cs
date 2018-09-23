using System;
using System.Collections.Generic;

public sealed class DisposableComponentManager : IDisposable
{
    private readonly List<IDisposable> disposableComponents = new List<IDisposable>();

    public void AddDisposable(IDisposable disposableComponent) => disposableComponents.Add(disposableComponent);

    public void Dispose() => disposableComponents.ForEach(disposable => disposable.Dispose());
}