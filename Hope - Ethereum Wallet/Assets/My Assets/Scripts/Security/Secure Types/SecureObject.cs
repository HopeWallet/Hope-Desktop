using System;
using System.Linq;
using System.Runtime.CompilerServices;

public abstract class SecureObject
{

    [ReflectionProtect(typeof(int))]
    [SecureCaller]
    public override int GetHashCode()
    {
        return IsSecureCall() ? base.GetHashCode() + RuntimeHelpers.GetHashCode(this) : default(int);
    }

    [ReflectionProtect(typeof(int))]
    [SecureCaller]
    public override string ToString()
    {
        return IsSecureCall() ? base.ToString() : null;
    }

    [ReflectionProtect(typeof(bool))]
    protected bool IsSecureCall()
    {
        RuntimeMethodSearcher.DisplayMethodCallStack();

        var methods = RuntimeMethodSearcher.WalkUntil<SecureCallEndAttribute>();
        var isSecureCall = methods != null && methods.Count(method => Attribute.IsDefined(method, typeof(SecureCallerAttribute))) == methods.Count - 1;

        isSecureCall.Log();

        return isSecureCall;
    }
}