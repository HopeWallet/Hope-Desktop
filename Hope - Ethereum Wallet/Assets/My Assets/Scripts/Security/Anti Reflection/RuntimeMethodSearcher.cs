using System.Diagnostics;

public static class RuntimeMethodSearcher
{

    private static readonly string[] REFLECTION_CALLS = new string[] { "Invoke", "InternalInvoke" };

    public static bool FindReflectionCalls()
    {
        StackTrace stackTrace = new StackTrace();

        for (int i = 0; ; i++)
        {
            string methodName = stackTrace.GetFrame(i)?.GetMethod()?.Name;

            if (methodName == null)
                return false;

            if (REFLECTION_CALLS.ContainsIgnoreCase(methodName))
                return true;
        }
    }

}