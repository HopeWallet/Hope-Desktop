using Hope.Security.Encryption.DPAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class ProtectedTypeBase<T>
{

    protected byte[] data;

    public bool IsLocked { get; private set; }

    public string EncryptedValue => data.GetBase64String();

    public T Value
    {
        get { return ConvertToType(MemProtect.Unprotect(data).GetUTF8String()); }
        set { data = MemProtect.Protect(value.ToString().GetUTF8Bytes()); }
    }

    public ProtectedTypeBase(T value)
    {
        Value = value;
    }

    protected abstract T ConvertToType(string strValue);

}