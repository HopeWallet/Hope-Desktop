using System;

/// <summary>
/// Attribute to be placed on methods which want protection against the potential to be called using .NET Reflection.
/// </summary>
//[AttributeUsage(AttributeTargets.Method)]
//public sealed class ReflectionProtectAttribute : Attribute
//{

//    private readonly Type returnType;

//    /// <summary>
//    /// The default value returned from the method.
//    /// </summary>
//    public object ReturnValue => returnType == null ? "" : returnType.IsValueType? Activator.CreateInstance(returnType) : "null";

//    /// <summary>
//    /// Initializes the ReflectionProtectAttribute with the type of the method return type.
//    /// </summary>
//    /// <param name="returnType"> The return type of the method attached to the Attribute. </param>
//    public ReflectionProtectAttribute(Type returnType)
//    {
//        this.returnType = returnType;
//    }

//    /// <summary>
//    /// Initializes the ReflectionProtectAttribute for a method with no return type.
//    /// </summary>
//    public ReflectionProtectAttribute()
//    {
//    }

//}