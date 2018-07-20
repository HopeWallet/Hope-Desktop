using System;

/// <summary>
/// Attribute which indicates this method is a secure caller. 
/// Should only be called by other SecureCaller attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class SecureCallerAttribute : Attribute
{
}