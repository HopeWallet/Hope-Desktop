using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Class which contains some utility methods for interacting with .net assemblies.
/// </summary>
public static class AssemblyUtils
{

    private const BindingFlags REFLECTION_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

    /// <summary>
    /// Gets a list of Types with a given method attribute.
    /// </summary>
    /// <typeparam name="T"> The attribute to get the types for. </typeparam>
    /// <returns> The types with the method attribute. </returns>
    public static List<Type> GetTypesWithMethodAttribute<T>() where T : Attribute
    {
        Assembly mainAssembly = AppDomain.CurrentDomain.GetAssemblies()
                                 .Single(assembly => assembly.GetName().Name.EqualsIgnoreCase("Assembly-CSharp"));

        List<Type> types = new List<Type>();

        foreach (Type type in mainAssembly.GetTypes())
        {
            try
            {
                foreach (var method in type.GetMethods(REFLECTION_FLAGS))
                    if (Attribute.IsDefined(method, typeof(T)) && !types.Contains(type))
                        types.Add(type);
            }
            catch
            {
            }
        }

        return types;
    }
}