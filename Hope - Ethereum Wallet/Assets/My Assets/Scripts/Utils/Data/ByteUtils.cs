using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Class which contains several utils for converting/manipulating byte data.
/// </summary>
public static class ByteUtils
{

    /// <summary>
    /// Gets a hexadecimal string from a collection of byte data.
    /// </summary>
    /// <param name="data"> The byte data to convert to a string. </param>
    /// <returns> The hexadecimal string. </returns>
    public static string ToHexString(this IEnumerable<byte> data)
    {
        StringBuilder sBuilder = new StringBuilder();

        foreach (byte b in data)
            sBuilder.Append(b.ToString("x2"));

        return sBuilder.ToString();
    }

    /// <summary>
    /// Converts a collection of byte data to a Base64 string.
    /// </summary>
    /// <param name="data"> The data to convert to Base64 string. </param>
    /// <returns> The Base64 string of the byte data. </returns>
    public static string ToBase64String(this IEnumerable<byte> data) => Convert.ToBase64String(data.ToArray());

}