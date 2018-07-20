using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Class which is used to search through the StackTrace at runtime and determine certain information about method calls.
/// </summary>
public static class RuntimeMethodSearcher
{

    private static readonly string ReflectionCall = "InternalInvoke";

    /// <summary>
    /// Determines if the calling method was called through reflection or not.
    /// </summary>
    /// <returns> True if any method in the line of methods calling this method was called with reflection. </returns>
    public static bool FindReflectionCalls()
    {
        StackTrace stackTrace = new StackTrace();

        for (int i = 1; ; i++)
        {
            string methodName = stackTrace.GetFrame(i)?.GetMethod()?.Name;

            if (methodName == null)
                return false;

            if (ReflectionCall.EqualsIgnoreCase(methodName))
                return true;
        }
    }

    /// <summary>
    /// Walks the stacktrace until an attribute is hit.
    /// </summary>
    /// <typeparam name="T"> The type of attribute to stop walking the stacktrace at. </typeparam>
    /// <returns> Null if the attribute is not found, otherwise returns the list of methods on the way to the attribute. </returns>
    public static List<MethodBase> WalkUntil<T>() where T : Attribute
    {
        StackTrace stackTrace = new StackTrace();
        List<MethodBase> methods = new List<MethodBase>();

        for (int i = 1; ; i++)
        {
            MethodBase method = stackTrace.GetFrame(i)?.GetMethod();

            if (method == null)
                return null;
            else if (Attribute.IsDefined(method, typeof(T)))
                return methods;

            methods.Add(method);
        }
    }

    /// <summary>
    /// Displays the methods that were called along the current hierarchy.
    /// </summary>
    public static void DisplayMethodCallStack()
    {
        StackTrace stackTrace = new StackTrace();
        List<string> methodCalls = new List<string>();

        for (int i = 0; ; i++)
        {
            string methodName = stackTrace.GetFrame(i)?.GetMethod()?.Name;

            if (methodName == null)
                break;

            methodCalls.Add(methodName);
        }

        methodCalls.Reverse();
        string.Join(" => ", methodCalls.ToArray()).Log();
    }
}