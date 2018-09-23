using System.Collections.Generic;
using Zenject;

/// <summary>
/// Class which handles all updating for updater components.
/// </summary>
public class UpdateManager : ITickable, ILateTickable
{
    private List<IUpdater> updaters = new List<IUpdater>();
    private List<ILateUpdater> lateUpdaters = new List<ILateUpdater>();

    /// <summary>
    /// Updates all IUpdaters.
    /// </summary>
    public void Tick() => lateUpdaters.SafeForEach(u => u.LateUpdaterUpdate());

    /// <summary>
    /// Updates all ILateUpdaters.
    /// </summary>
    public void LateTick() => updaters.SafeForEach(u => u.UpdaterUpdate());

    /// <summary>
    /// Adds an IUpdater to the list of updaters.
    /// </summary>
    /// <param name="updater"> The IUpdater to add. </param>
    public void AddUpdater(IUpdater updater) => updaters.Add(updater);

    /// <summary>
    /// Removes an IUpdater from the list of updaters.
    /// </summary>
    /// <param name="updater"> The IUpdater to remove. </param>
    public void RemoveUpdater(IUpdater updater) => updaters.Remove(updater);

    /// <summary>
    /// Adds an ILateUpdater to the list of late updaters.
    /// </summary>
    /// <param name="updater"> The ILateUpdater to add. </param>
    public void AddLateUpdater(ILateUpdater updater) => lateUpdaters.Add(updater);

    /// <summary>
    /// Removes an ILateUpdater from the list of late updaters.
    /// </summary>
    /// <param name="updater"> The ILateUpdater to remove. </param>
    public void RemoveLateUpdater(ILateUpdater updater) => lateUpdaters.Remove(updater);

}
