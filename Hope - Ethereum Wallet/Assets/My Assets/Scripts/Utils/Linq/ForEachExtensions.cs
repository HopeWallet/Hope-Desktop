using System;
using System.Collections.Generic;

/// <summary>
/// Class with basic extensions for any collection classes.
/// </summary>
public static class ForEachExtensions
{

    /// <summary>
    /// Interates through each element of a list and performs an action with each element.
    /// Uses a regular for loop to avoid foreach errors when removing elements from a list at runtime.
    /// </summary>
    /// <typeparam name="T"> The type of element in the list. </typeparam>
    /// <param name="collection"> The list to iterate through. </param>
    /// <param name="action"> The action to execute with each element. </param>
    public static void SafeForEach<T>(this IList<T> list, Action<T> action)
    {
        for (int i = 0; i < list.Count; i++)
            action(list[i]);
    }

    /// <summary>
    /// Interates through each element of an enumerable and performs an action with each element.
    /// </summary>
    /// <typeparam name="T"> The type of element in the collection. </typeparam>
    /// <param name="enumerable"> The enumerable to iterate through. </param>
    /// <param name="action"> The action to execute with each element. </param>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (T element in enumerable)
            action(element);
    }

}
