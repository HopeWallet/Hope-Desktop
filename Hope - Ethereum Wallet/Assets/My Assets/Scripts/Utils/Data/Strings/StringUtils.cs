using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

/// <summary>
/// Class which contains certain <see langword="string"/> utilities.
/// </summary>
public static class StringUtils
{
    /// <summary>
    /// Checks if two strings characters are equal, ignoring uppercase or lowercase differences.
    /// </summary>
    /// <param name="str1"> The first <see langword="string"/> to compare. </param>
    /// <param name="str2"> The second <see langword="string"/> to compare. </param>
    /// <param name="trimEmptyChars"> Trims the empty spaces and characters when comparing the <see langword="string"/> values. </param>
    /// <returns> Whether the two <see langword="string"/> values are equal. </returns>
    public static bool EqualsIgnoreCase(this string str1, string str2, bool trimEmptyChars = false) 
        => string.Equals(trimEmptyChars ? str1.Trim() : str1, trimEmptyChars ? str2.Trim() : str2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Trims the end of a string if it is past a certain length, and adds a certain string to the end if it was over the length.
    /// </summary>
    /// <param name="str"> The string to check. </param>
    /// <param name="maxLength"> The maximum length of this string. </param>
    /// <param name="endCharacters"> The characters to add to the end of the string if it is over the maximum length. </param>
    /// <returns> The trimmed string if it was over the maximum length, otherwise the same string. </returns>
    public static string LimitEnd(this string str, int maxLength, string endCharacters = "") => string.IsNullOrEmpty(str) ? string.Empty : str.Length <= maxLength ? str : str.Substring(0, maxLength) + endCharacters;

    /// <summary>
    /// Checks if there is a string in a collection that matches with another string, ignoring case sensitivity.
    /// </summary>
    /// <param name="stringCollection"> The string collection to check through. </param>
    /// <param name="str"> The string to check for. </param>
    /// <param name="trimEmptyChars"> Trims the empty spaces and characters when comparing the strings. </param>
    /// <returns> Whether the string collection contains the string. </returns>
    public static bool ContainsIgnoreCase(this IEnumerable<string> stringCollection, string str, bool trimEmptyChars = false)
    {
        foreach (string s in stringCollection)
            if (str.EqualsIgnoreCase(s, trimEmptyChars))
                return true;

        return false;
    }

    /// <summary>
    /// Converts a <see langword="decimal"/> value to a <see langword="string"/> value.
    /// </summary>
    /// <param name="value"> The <see langword="decimal"/> value to convert to a <see langword="string"/>. </param>
    /// <returns> The <see langword="string"/> representation of the value. </returns>
    public static string ConvertDecimalToString(this decimal value)
    {
        string decimalString = value.ToString();
        return !decimalString.Contains(".") ? decimalString : decimalString.TrimEnd('0').TrimEnd('.');
    }

    /// <summary>
    /// Converts a Base64 string to a byte array.
    /// </summary>
    /// <param name="str"> The Base64 string to convert. </param>
    /// <returns> The byte data of the string. </returns>
    public static byte[] GetBase64Bytes(this string str) => string.IsNullOrEmpty(str) ? null : Convert.FromBase64String(str);

    /// <summary>
    /// Converts string to a byte array using UTF8 encoding.
    /// </summary>
    /// <param name="str"> The string to encode. </param>
    /// <returns> The string encoded to UTF8 bytes. </returns>
    public static byte[] GetUTF8Bytes(this string str) => string.IsNullOrEmpty(str) ? null : Encoding.UTF8.GetBytes(str);

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
    /// <returns> The string split in two parts. </returns>
    public static Tuple<string, string> SplitHalf(this string str)
    {
        var length = str.Length;
        var halfLength = str.Length / 2;
        var fixedLength = length % 2 == 1 ? halfLength + 1 : halfLength;

        return new Tuple<string, string>(str.Substring(0, halfLength), str.Substring(halfLength, fixedLength));
    }
}