using Hope.Security.Encryption.DPAPI;
using Hope.Security.Encryption.Symmetric;
using System;

/// <summary>
/// Class which encrypts data which can be decrypted over several sessions of lifetime.
/// </summary>
public static class EternalEncryption
{
    private static bool IsWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;

    public static string Encrypt(string data, string entropy)
    {
        return Encrypt(data, entropy.GetUTF8Bytes());
    }

    public static string Encrypt(string data, byte[] entropy)
    {
        return Encrypt(data.GetUTF8Bytes(), entropy).GetBase64String();
    }

    public static byte[] Encrypt(byte[] data, string entropy)
    {
        return Encrypt(data, entropy.GetUTF8Bytes());
    }

    public static byte[] Encrypt(byte[] data, byte[] entropy)
    {
        return InternalEncrypt(data, entropy);
    }

    public static string Decrypt(string data, string entropy)
    {
        return Decrypt(data, entropy.GetUTF8Bytes());
    }

    public static string Decrypt(string data, byte[] entropy)
    {
        return Decrypt(data.GetBase64Bytes(), entropy).GetUTF8String();
    }

    public static byte[] Decrypt(byte[] data, string entropy)
    {
        return Decrypt(data, entropy.GetUTF8Bytes());
    }

    public static byte[] Decrypt(byte[] data, byte[] entropy)
    {
        return InternalDecrypt(data, entropy);
    }

    public static byte[] InternalEncrypt(byte[] data, byte[] entropy)
    {
        return null;
    }

    public static byte[] InternalDecrypt(byte[] data, byte[] entropy)
    {
        return null;
    }
}