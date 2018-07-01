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
    public static string GetHexString(this IEnumerable<byte> data)
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
    public static string GetBase64String(this IEnumerable<byte> data) => data.IsInvalid() ? null : Convert.ToBase64String(data.ToArray());

    /// <summary>
    /// Converts a collection of byte data to a string using UTF8 encoding.
    /// </summary>
    /// <param name="data"> The data to convert to a UTF8 string. </param>
    /// <returns> The string converted from UTF8 format. </returns>
    public static string GetUTF8String(this IEnumerable<byte> data) => data.IsInvalid() ? null : Encoding.UTF8.GetString(data.ToArray());

    /// <summary>
    /// Checks if a collection of byte data is invalid to encode to a string.
    /// </summary>
    /// <param name="data"> The byte data to perform the check on. </param>
    /// <returns> True if the byte data is invalid. </returns>
    private static bool IsInvalid(this IEnumerable<byte> data) => data?.Any() != true;

}