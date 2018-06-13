/// <summary>
/// Struct which represents a string that was split in half.
/// </summary>
public struct SplitString
{

    public string firstHalf;
    public string secondHalf;

    /// <summary>
    /// Initializes the string with the first half and second half.
    /// </summary>
    /// <param name="firstHalf"> The first half of the string. </param>
    /// <param name="secondHalf"> The second half of the string. </param>
    public SplitString(string firstHalf, string secondHalf)
    {
        this.firstHalf = firstHalf;
        this.secondHalf = secondHalf;
    }
}
