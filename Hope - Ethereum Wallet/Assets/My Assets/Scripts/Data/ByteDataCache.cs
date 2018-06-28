using System.Collections.Generic;

/// <summary>
/// Class which contains byte data for use in different scenarios.
/// </summary>
public class ByteDataCache
{
    /// <summary>
    /// List of collective byte data.
    /// </summary>
    public List<byte[]> CollectiveData { get; } = new List<byte[]>();

    /// <summary>
    /// List of singular byte data.
    /// </summary>
    public List<byte> SingularData { get; } = new List<byte>();

}