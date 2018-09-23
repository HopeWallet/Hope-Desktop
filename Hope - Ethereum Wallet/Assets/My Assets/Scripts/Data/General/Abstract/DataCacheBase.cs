using Hope.Security.ProtectedTypes.Types;
using System.Collections.Generic;

/// <summary>
/// Class which contains data for use in different scenarios.
/// </summary>
/// <typeparam name="T"> The data type of the cache. </typeparam>
public abstract class DataCache<T>
{
    private readonly Dictionary<string, T> data = new Dictionary<string, T>();

    /// <summary>
    /// Clears all data in the cache.
    /// </summary>
    public void ClearAllData()
    {
        foreach (var pair in data)
        {
            if (pair.Value is ProtectedString)
                (pair.Value as ProtectedString)?.Dispose();
        }

        data.Clear();
    }

    /// <summary>
    /// Sets a value to the DataCache with the key.
    /// </summary>
    /// <param name="key"> The key to use to retrieve the value. </param>
    /// <param name="value"> The value to set. </param>
    public void SetData(string key, T value)
    {
        if (data.ContainsKey(key))
            data[key] = value;
        else
            data.Add(key, value);
    }

    /// <summary>
    /// Gets some data from the DataCache given the key.
    /// </summary>
    /// <param name="key"> The key to use to access the data. </param>
    /// <returns> The data retrieved from the cache. </returns>
    public T GetData(string key)
    {
        return data.ContainsKey(key) ? data[key] : default(T);
    }
}