using System.Security.Cryptography;
using System;

public sealed class HopeDataProtector : ProtectorBase
{
    public HopeDataProtector(params object[] protectorObjects) : base(protectorObjects)
    {
    }

    protected override void InternalProtect(byte[] data, byte[] entropy)
    {
        throw new NotImplementedException();
    }

    protected override void InternalUnprotect(byte[] data, byte[] entropy)
    {
        throw new NotImplementedException();
    }
}