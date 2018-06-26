using UnityEngine;

/// <summary>
/// Class which contains certain utility methods for displaying debug messages easily.
/// </summary>
public static class DebugUtils
{

    /// <summary>
    /// Debug.Log called on the input object.
    /// </summary>
    /// <param name="obj"> The object to debug. </param>
    public static void Log(this object obj) => Debug.Log(obj.ToString());

    /// <summary>
    /// Condenses the contents of an array into one string and displays it in the console.
    /// </summary>
    /// <typeparam name="T"> The type of the array to print. </typeparam>
    /// <param name="array"> The array to display. </param>
    public static void LogArray<T>(this T[] array)
    {
        string text = "";
        array.ForEach(i => text += i.ToString() + ", ");
        Debug.Log(text.Remove(text.Length - 2, 1));
    }

}