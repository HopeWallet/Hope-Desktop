using System;

/// <summary>
/// Attribute to be placed on private methods which want protection against the potential to be called using .NET Reflection.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ReflectionProtectAttribute : Attribute
{

    private Type returnType;

    public object ReturnValue => returnType == null ? "" : returnType.IsValueType? Activator.CreateInstance(returnType) : null;

    public ReflectionProtectAttribute(Type returnType)
    {
        this.returnType = returnType;
    }

    public ReflectionProtectAttribute()
    {
    }

}