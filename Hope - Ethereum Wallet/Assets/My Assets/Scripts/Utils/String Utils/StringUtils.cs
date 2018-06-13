using System;
using System.Numerics;

/// <summary>
/// Class which contains certain string utilities.
/// </summary>
public static class StringUtils
{

    /// <summary>
    /// Checks if two strings characters are equal, ignoring uppercase or lowercase differences.
    /// </summary>
    /// <param name="str1"> The first string to compare. </param>
    /// <param name="str2"> The second string to compare. </param>
    /// <returns> Whether the two strings are equal. </returns>
    public static bool EqualsIgnoreCase(this string str1, string str2) => string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Trims the end of a string if it is past a certain length, and adds a certain string to the end if it was over the length.
    /// </summary>
    /// <param name="str"> The string to check. </param>
    /// <param name="maxLength"> The maximum length of this string. </param>
    /// <param name="endCharacters"> The characters to add to the end of the string if it is over the maximum length. </param>
    /// <returns> The trimmed string if it was over the maximum length, otherwise the same string. </returns>
    public static string LimitEnd(this string str, int maxLength, string endCharacters = "") => str.Length <= maxLength ? str : str.Substring(0, maxLength) + endCharacters;

    /// <summary>
    /// Iterates through each element of the string and performs an action with each character.
    /// </summary>
    /// <param name="str"> The string to iterate through. </param>
    /// <param name="action"> The action to perform with each character. </param>
    public static void Foreach(this string str, Action<char> action)
    {
        foreach (char c in str)
            action?.Invoke(c);
    }

    /// <summary>
    /// Converts a string from hex to a decimal value.
    /// Supports very large hexadecimal numbers.
    /// </summary>
    /// <param name="hexStr"> The string to convert from hex to decimal. </param>
    /// <returns> The decimal value of the hex string. </returns>
    public static BigInteger ConvertFromHex(this string hexStr)
    {
        hexStr = hexStr.TrimStart('0', 'x');

        BigInteger sum = new BigInteger();
        int strLen = hexStr.Length;

        for (int i = 0; i < strLen; i++)
        {
            var val = int.Parse(hexStr[i].ToString(), System.Globalization.NumberStyles.HexNumber);

            if (val > 16)
                return sum;
            
            sum += val * BigInteger.Pow(16, (strLen - 1) - i);
        }

        return sum;
    }

    /// <summary>
    /// Splits a string in half and returns a SplitString of the input string.
    /// </summary>
    /// <param name="str"> The string to split. </param>
    /// <returns> The SplitString created from the string. </returns>
    public static SplitString SplitHalf(this string str)
    {
        var length = str.Length;
        var halfLength = str.Length / 2;
        var fixedLength = length % 2 == 1 ? halfLength + 1 : halfLength;

        return new SplitString(str.Substring(0, halfLength), str.Substring(halfLength, fixedLength));
    }

}