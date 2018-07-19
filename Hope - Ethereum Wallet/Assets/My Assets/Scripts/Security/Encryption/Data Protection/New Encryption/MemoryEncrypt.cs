using Hope.Security.HashGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public class MemoryEncrypt
{

    public string GetEncryptionPassword()
    {
        return GetEncryptionPassword("");
    }

    [ReflectionProtect(typeof(string))]
    public string GetEncryptionPassword(string entropy)
    {
        return GetHashCode().ToString().GetSHA256Hash()
                    .CombineAndRandomize(GetProcessId().ToString().GetSHA256Hash())
                    .CombineAndRandomize(GetRuntimeHashCode().ToString().GetSHA384Hash()
                    .CombineAndRandomize(GetProcessModuleHashCode().ToString().GetSHA384Hash())
                    .CombineAndRandomize(entropy).GetSHA256Hash()).GetSHA512Hash();
    }

    //[ReflectionProtect(typeof(int))]
    //public override int GetHashCode()
    //{
    //    return base.GetHashCode();
    //}

    [ReflectionProtect(typeof(int))]
    private new int GetHashCode()
    {
        UnityEngine.Debug.Log("getting hash code");
        return base.GetHashCode();
    }

    [ReflectionProtect(typeof(int))]
    private int GetRuntimeHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    [ReflectionProtect(typeof(int))]
    private int GetProcessId()
    {
        return Process.GetCurrentProcess().Id;
    }

    [ReflectionProtect(typeof(int))]
    private int GetProcessModuleHashCode()
    {
        return Process.GetCurrentProcess().MainModule.ModuleName.GetHashCode();
    }

}