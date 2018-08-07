using System;

/// <summary>
/// Attribute to apply to fields that need to be changed once the build is created.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class RandomizeTextAttribute : Attribute
{
}