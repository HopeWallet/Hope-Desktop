using Hope.Security.HashGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

/// <summary>
/// Class which is used to search through the StackTrace at runtime and determine certain information about method calls.
/// </summary>
public static class RuntimeMethodSearcher
{

    private static readonly StackTrace StackTrace = new StackTrace();

    private static readonly string ReflectionCall = "InternalInvoke";

    /// <summary>
    /// Determines if the calling method was called through reflection or not.
    /// </summary>
    /// <returns> True if any method in the line of methods calling this method was called with reflection. </returns>
    public static bool FindReflectionCalls()
    {
        for (int i = 0; ; i++)
        {
            string methodName = StackTrace.GetFrame(i)?.GetMethod()?.Name;

            if (methodName == null)
                return false;

            if (ReflectionCall.EqualsIgnoreCase(methodName))
                return true;
        }
    }

    /// <summary>
    /// Displays the methods that were called along the current hierarchy.
    /// </summary>
    public static void DisplayMethodCallStack()
    {
        List<string> methodCalls = new List<string>();
        for (int i = 0; ; i++)
        {
            string methodName = StackTrace.GetFrame(i)?.GetMethod()?.Name;

            if (methodName == null)
                break;

            methodCalls.Add(methodName);
        }

        methodCalls.Reverse();
        string.Join(" => ", methodCalls.ToArray()).Log();
    }
}