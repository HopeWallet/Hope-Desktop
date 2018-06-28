using System.Collections.Generic;

/// <summary>
/// Class which contains data for use in different scenarios.
/// </summary>
public abstract class DataCache<T>
{
    /// <summary>
    /// List of collective data.
    /// </summary>
    public List<T[]> CollectiveData { get; } = new List<T[]>();

    /// <summary>
    /// List of singular data.
    /// </summary>
    public List<T> SingularData { get; } = new List<T>();

}