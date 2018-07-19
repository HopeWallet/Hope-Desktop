using Hope.Security.HashGeneration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

public class MemoryEncrypt
{

    public string Encrypt(string data)
    {
        return Encrypt(data.GetUTF8Bytes()).GetBase64String();
    }

    public byte[] Encrypt(byte[] data)
    {
        return Encrypt(data, string.Empty);
    }

    public string Encrypt(string data, string entropy)
    {
        return Encrypt(data.GetUTF8Bytes(), entropy).GetBase64String();
    }

    public byte[] Encrypt(byte[] data, string entropy)
    {
        using (var rsa = GetRSA(entropy))
            return rsa.Encrypt(data, true);
    }

    public string Decrypt(string encryptedText)
    {
        return Decrypt(encryptedText.GetBase64Bytes()).GetUTF8String();
    }

    public byte[] Decrypt(byte[] encryptedData)
    {
        return Decrypt(encryptedData, string.Empty);
    }

    public string Decrypt(string encryptedText, string entropy)
    {
        return Decrypt(encryptedText.GetBase64Bytes(), entropy).GetUTF8String();
    }

    public byte[] Decrypt(byte[] encryptedData, string entropy)
    {
        byte[] decryptedData;
        using (var rsa = GetRSA(entropy))
            decryptedData = rsa.Decrypt(encryptedData, true);

        return decryptedData;
    }

    [ReflectionProtect(typeof(string))]
    private string GetEncryptionPassword(string entropy)
    {
        return GetHashCode().ToString().GetSHA256Hash()
                    .CombineAndRandomize(GetProcessId().ToString().GetSHA256Hash())
                    .CombineAndRandomize(GetRuntimeHashCode().ToString().GetSHA384Hash()
                    .CombineAndRandomize(GetProcessModuleHashCode().ToString().GetSHA384Hash())
                    .CombineAndRandomize(entropy).GetSHA256Hash()).GetSHA512Hash();
    }

    [ReflectionProtect(typeof(int))]
    private new int GetHashCode()
    {
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

    [ReflectionProtect(typeof(RSACryptoServiceProvider))]
    private RSACryptoServiceProvider GetRSA(string rsaEntropy)
    {
        return new RSACryptoServiceProvider(1024, new CspParameters(1, null, GetEncryptionPassword(rsaEntropy)) { Flags = CspProviderFlags.UseUserProtectedKey });
    }

    [ReflectionProtect]
    private void DeleteRSAKeys(string rsaEntropy)
    {
        var rsa = GetRSA(rsaEntropy);
        rsa.PersistKeyInCsp = false;
        rsa.Clear();
    }

}