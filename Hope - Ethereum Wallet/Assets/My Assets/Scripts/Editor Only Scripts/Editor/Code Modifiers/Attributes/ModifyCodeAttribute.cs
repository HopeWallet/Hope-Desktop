using System;

/// <summary>
/// Attribute used for methods that modify code in the project.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ModifyCodeAttribute : Attribute
{
}
