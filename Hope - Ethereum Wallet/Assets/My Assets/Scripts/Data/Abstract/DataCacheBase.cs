using System.Collections.Generic;

/// <summary>
/// Class which contains data for use in different scenarios.
/// </summary>
public abstract class DataCache<T>
{

    private readonly List<T> dataCollection = new List<T>();

    /// <summary>
    /// Sets the data cache at a given index.
    /// </summary>
    /// <param name="value"> The value of the data to set. </param>
    /// <param name="index"> The index to set the data at. If the index is outside the range, simply add the data onto the end of the list. </param>
    public void SetData(T value, int index)
    {
        if (dataCollection.Count <= index)
            dataCollection.Add(value);
        else
            dataCollection[index] = value;
    }

    /// <summary>
    /// Gets the data from the cache at a given index.
    /// </summary>
    /// <param name="index"> The index of the data. </param>
    /// <returns> The data found at the given index. </returns>
    public T GetData(int index)
    {
        if (index >= dataCollection.Count)
            return default(T);

        return dataCollection[index];
    }

}