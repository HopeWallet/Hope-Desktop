using System.Collections.Generic;

/// <summary>
/// Class which contains data for use in different scenarios.
/// </summary>
public abstract class DataCache<T>
{
    /// <summary>
    /// Gets the data at the index of the CollectiveData list.
    /// </summary>
    /// <param name="index"> The index to get the data from. </param>
    /// <returns> The data array. </returns>
    public T[] this[int index]
    {
        get
        {
            return CollectiveData[index];
        }
        set
        {
            if (CollectiveData.Count <= index)
                CollectiveData.Add(value);
            else
                CollectiveData[index] = value;
        }
    }

    /// <summary>
    /// List of collective data.
    /// </summary>
    public List<T[]> CollectiveData { get; } = new List<T[]>();

    /// <summary>
    /// List of singular data.
    /// </summary>
    public List<T> SingularData { get; } = new List<T>();

}