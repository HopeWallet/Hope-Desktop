using Org.BouncyCastle.Security;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Class which overrides the default object methods which restrict calls to only SecureCallers and non reflection calls.
/// </summary>
public abstract class SecureObject
{
    private readonly SecureRandom random = new SecureRandom();

    /// <summary>
    /// Gets the hashcode of this object.
    /// Cannot get the hashcode if the calling method is not secure or if called through reflection.
    /// </summary>
    /// <returns> The runtime and base hashcode of this object. </returns>
    [SecureCaller]
    [ReflectionProtect(typeof(int))]
    public override int GetHashCode()
    {
        return IsSecureCall() ? base.GetHashCode() + RuntimeHelpers.GetHashCode(this) : random.NextInt();
    }

    /// <summary>
    /// Gets the string representation of this object.
    /// Cannot get the string representation if the calling method is not secure or if called through reflection.
    /// </summary>
    /// <returns> The string representation of this object. </returns>
    [SecureCaller]
    [ReflectionProtect(typeof(int))]
    public override string ToString()
    {
        return IsSecureCall() ? base.ToString() : SecureRandom.GetNextBytes(random, 12).GetBase64String();
    }

    /// <summary>
    /// Gets the type of this object.
    /// Cannot get the type if the calling method is not secure or if called through reflection.
    /// </summary>
    /// <returns> The type of this object. </returns>
    [SecureCaller]
    [ReflectionProtect(typeof(Type))]
    public new Type GetType()
    {
        return IsSecureCall() ? base.GetType() : null;
    }

    /// <summary>
    /// Checks if the caller methods are secure.
    /// </summary>
    /// <returns> True if the line of methods calling this method are secure. </returns>
    [ReflectionProtect(typeof(bool))]
    protected bool IsSecureCall()
    {
        var methods = RuntimeMethodSearcher.WalkUntil<SecureCallEndAttribute>();
        return methods != null && methods.Count(method => Attribute.IsDefined(method, typeof(SecureCallerAttribute))) == methods.Count - 1;
    }
}