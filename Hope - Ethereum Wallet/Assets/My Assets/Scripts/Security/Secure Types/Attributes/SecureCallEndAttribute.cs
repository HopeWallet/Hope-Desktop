using System;

/// <summary>
/// Attribute which indicates the end of a line of SecureCallerAttributes on methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class SecureCallEndAttribute : Attribute
{
}