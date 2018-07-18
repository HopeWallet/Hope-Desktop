using System;

/// <summary>
/// Attribute to be placed on private methods which want protection against the potential to be called using .NET Reflection.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ReflectionProtect : Attribute
{
}