using System;
using System.Threading;

/// <summary>
/// Class to derive from for any singleton class.
/// </summary>
/// <typeparam name="T"> The type of the singleton class. </typeparam>
public abstract class PureSingleton<T> where T : PureSingleton<T>
{

    private static readonly ThreadLocal<T> lazy = new ThreadLocal<T>(() => Activator.CreateInstance(typeof(T), true) as T);

    /// <summary>
    /// Gets the instance of the singleton object.
    /// </summary>
    public static T Instance => lazy.Value;

}
