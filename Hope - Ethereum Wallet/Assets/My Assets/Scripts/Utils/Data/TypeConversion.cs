using System.ComponentModel;

/// <summary>
/// Class which contains useful utility methods for converting certain object values to their specific type representation.
/// </summary>
public static class TypeConversion
{

    /// <summary>
    /// Changes the type of an object to the type of T.
    /// </summary>
    /// <typeparam name="T"> The type to have the value changed to. </typeparam>
    /// <param name="value"> The value to change the type of. </param>
    /// <returns> The value converted from object to type T. </returns>
    public static T ChangeType<T>(object value) => typeof(T) == typeof(object) ? (T)value : Convert<T>(value);

    /// <summary>
    /// Converts an object to type T.
    /// </summary>
    /// <typeparam name="T"> The type to convert to. </typeparam>
    /// <param name="value"> The value to convert. </param>
    /// <returns> The converted value. </returns>
    private static T Convert<T>(object value) => (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);

}