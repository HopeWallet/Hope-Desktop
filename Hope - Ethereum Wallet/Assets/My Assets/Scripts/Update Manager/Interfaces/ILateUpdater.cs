/// <summary>
/// Interface for components that need to be called during LateUpdate.
/// </summary>
public interface ILateUpdater
{
    /// <summary>
    /// Updates the component during LateUpdate.
    /// </summary>
    void LateUpdaterUpdate();
}
