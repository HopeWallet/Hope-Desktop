/// <summary>
/// Class which contains string data.
/// </summary>
public sealed class StringContainer : DataContainer<string>
{
    /// <summary>
    /// Initializes the DataContainer with the string data.
    /// </summary>
    /// <param name="value"> The string value. </param>
    public StringContainer(string value) : base(value)
    {
    }
}