using System;

/// <summary>
/// Attribute used on methods which restore changes to the project code.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RestoreCodeAttribute : Attribute
{
}
