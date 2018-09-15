using System.Collections.Generic;

/// <summary>
/// Class which contains certain extension methods for a list.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Adds an array of items to the list.
    /// </summary>
    /// <typeparam name="T"> The type of the list. </typeparam>
    /// <param name="list"> The list to add the items to. </param>
    /// <param name="items"> The items to add to the list. </param>
    public static List<T> AddItems<T>(this List<T> list, params T[] items)
    {
        items.ForEach(item => list.Add(item));
        return list;
    }
}