using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Class which contains several utils for converting/manipulating <see langword="byte"/>[] data.
/// </summary>
public static class ByteUtils
{

    /// <summary>
    /// Gets a hexadecimal <see langword="string"/> from a collection of <see langword="byte"/>[] data.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to convert to a <see langword="string"/>. </param>
    /// <returns> The hexadecimal <see langword="string"/>. </returns>
    public static string GetHexString(this IEnumerable<byte> data) => string.Concat(data.Select(b => b.ToString("x2")).ToArray());

    /// <summary>
    /// Converts a collection of <see langword="byte"/>[] data to a Base64 <see langword="string"/>.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to convert to Base64 <see langword="string"/>. </param>
    /// <returns> The Base64 <see langword="string"/> of the <see langword="byte"/>[] data. </returns>
    public static string GetBase64String(this IEnumerable<byte> data) => data.IsInvalid() ? null : Convert.ToBase64String(data.ToArray());

    /// <summary>
    /// Converts a collection of <see langword="byte"/>[] data to a <see langword="string"/> using UTF8 encoding.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to convert to a UTF8 <see langword="string"/>. </param>
    /// <returns> The <see langword="string"/> converted from UTF8 format. </returns>
    public static string GetUTF8String(this IEnumerable<byte> data) => data.IsInvalid() ? null : Encoding.UTF8.GetString(data.ToArray());

    /// <summary>
    /// Checks if a collection of <see langword="byte"/>[] data is invalid to encode to a <see langword="string"/>.
    /// </summary>
    /// <param name="data"> The <see langword="byte"/>[] data to perform the check on. </param>
    /// <returns> True if the <see langword="byte"/>[] data is invalid. </returns>
    private static bool IsInvalid(this IEnumerable<byte> data) => data?.Any() != true;

}