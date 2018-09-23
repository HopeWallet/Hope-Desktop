using System.Collections.Generic;

/// <summary>
/// Class which updates IPeriodicUpdaters repeatedly in periodic intervals.
/// </summary>
public sealed class PeriodicUpdateManager : IUpdater
{
    private readonly Dictionary<IPeriodicUpdater, KeyValuePair<float, bool>> periodicUpdaters;

    /// <summary>
    /// Initializes the PeriodicUpdateManager by creating the dictionary and adding itself to the UpdateManager.
    /// </summary>
    /// <param name="updateManager"> The UpdateManager to run the periodic updates off of. </param>
    public PeriodicUpdateManager(UpdateManager updateManager)
    {
        periodicUpdaters = new Dictionary<IPeriodicUpdater, KeyValuePair<float, bool>>();
        updateManager.AddUpdater(this);
    }

    /// <summary>
    /// Iterates through all elements of the dictionary and updates an element if the time has expired.
    /// </summary>
    public void UpdaterUpdate()
    {
        new List<IPeriodicUpdater>(periodicUpdaters.Keys).ForEach(updater =>
        {
            var pair = periodicUpdaters[updater];

            CheckUpdaterStatus(updater, pair.Key, pair.Value);
        });
    }

    /// <summary>
    /// Adds an IUpdater to the dictionary of IPeriodicUpdaters to periodically update.
    /// </summary>
    /// <param name="updater"> The IPeriodicUpdater to update in given intervals. </param>
    /// <param name="firstUpdateNow"> Whether the first PeriodicUpdate should occur now. </param>
    public void AddPeriodicUpdater(IPeriodicUpdater updater, bool firstUpdateNow = false)
    {
        if (firstUpdateNow)
            updater.PeriodicUpdate();

        periodicUpdaters[updater] = new KeyValuePair<float, bool>(updater.UpdateInterval, false);
    }

    /// <summary>
    /// Removes an IUpdater from the dictionary of IPeriodicUpdaters to periodically update.
    /// </summary>
    /// <param name="updater"> The IPeriodicUpdater to remove. </param>
    public void RemovePeriodicUpdater(IPeriodicUpdater updater)
    {
        if (!periodicUpdaters.ContainsKey(updater))
            return;

        periodicUpdaters.Remove(updater);
    }

    /// <summary>
    /// Checks on the status of an IPeriodicUpdater, and begins the wait if it has recently finished updating.
    /// </summary>
    /// <param name="updater"> The IPeriodicUpdater to check. </param>
    /// <param name="waitTime"> The wait interval of the the IUpdater. </param>
    /// <param name="currentlyUpdating"> Whether the IPeriodicUpdater is currently updating or not. </param>
    private void CheckUpdaterStatus(IPeriodicUpdater updater, float waitTime, bool currentlyUpdating)
    {
        if (currentlyUpdating)
            return;

        periodicUpdaters[updater] = new KeyValuePair<float, bool>(waitTime, true);
        CoroutineUtils.ExecuteAfterWait(waitTime, () => OnTimeWaited(updater, waitTime));
    }

    /// <summary>
    /// Executed once the time has been waited, which executes the updaters update method.
    /// </summary>
    /// <param name="updater"> The updater to update. </param>
    /// <param name="waitTime"> The time interval between each update. </param>
    private void OnTimeWaited(IPeriodicUpdater updater, float waitTime)
    {
        if (!periodicUpdaters.ContainsKey(updater))
            return;

        periodicUpdaters[updater] = new KeyValuePair<float, bool>(waitTime, false);
        updater.PeriodicUpdate();
    }

}
