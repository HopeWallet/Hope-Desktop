
/// <summary>
/// Interface for all classes which require a periodic update.
/// </summary>
public interface IPeriodicUpdater
{

    /// <summary>
    /// The update interval of the IPeriodicUpdater.
    /// </summary>
    float UpdateInterval { get; }

    /// <summary>
    /// Repeatedly called after the time interval has been waited.
    /// </summary>
    void PeriodicUpdate();
}