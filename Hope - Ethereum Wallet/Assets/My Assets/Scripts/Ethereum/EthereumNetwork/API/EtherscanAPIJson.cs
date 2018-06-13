using System;

/// <summary>
/// Class which represents a json object of the list of transactions received from an address.
/// </summary>
[Serializable]
public class EtherscanAPIJson<T>
{

    public int status;
    public string message;
    public T[] result;

}
