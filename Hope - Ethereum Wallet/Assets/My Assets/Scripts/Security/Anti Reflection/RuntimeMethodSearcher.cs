using System.Diagnostics;

/// <summary>
/// Class which is used to search through the StackTrace at runtime and determine certain information about method calls.
/// </summary>
public static class RuntimeMethodSearcher
{

    private static readonly StackTrace StackTrace = new StackTrace();

    private static readonly string[] ReflectionCalls = new string[] { "Invoke", "InternalInvoke" };

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

            if (ReflectionCalls.ContainsIgnoreCase(methodName))
                return true;
        }
    }

}