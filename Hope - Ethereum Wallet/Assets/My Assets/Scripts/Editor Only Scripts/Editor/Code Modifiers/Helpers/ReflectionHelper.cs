using System;

/// <summary>
/// Class which has some helpful reflection methods that relate to attributes.
/// </summary>
public static class ReflectionHelper
{

    /// <summary>
    /// Invokes all static methods of a certain type if it contains a specific attribute.
    /// </summary>
    /// <typeparam name="T"> The type of attribute to invoke methods for. </typeparam>
    /// <param name="type"> The class type to search for attributes. </param>
    /// <param name="parameters"> The method parameters of the methods to invoke. </param>
    public static void InvokeAttributeMethod<T>(Type type, object[] parameters) where T : Attribute
    {
        foreach (var method in type.GetMethods())
            if (Attribute.IsDefined(method, typeof(T)))
                method.Invoke(null, parameters);
    }

    /// <summary>
    /// Checks a type if it contains methods with two different attributes.
    /// </summary>
    /// <typeparam name="T"> The type of the first attribute to check for. </typeparam>
    /// <typeparam name="V"> The type of the second attribute to check for. </typeparam>
    /// <param name="type"> The class type to search for methods with the attribute types. </param>
    /// <returns> True if the class type contains methods with all attribute types. </returns>
    public static bool HasMethodAttributes<T, V>(Type type)
    {
        bool hasT = false, hasV = false;

        foreach(var method in type.GetMethods())
        {
            hasT = Attribute.IsDefined(method, typeof(T)) || hasT;
            hasV = Attribute.IsDefined(method, typeof(V)) || hasV;
        }

        return hasT && hasV;
    }
}
