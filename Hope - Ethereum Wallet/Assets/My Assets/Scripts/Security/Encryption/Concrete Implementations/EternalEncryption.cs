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
    }

    public static string Encrypt(string data, byte[] entropy)
    {
    }

    public static byte[] Encrypt(byte[] data, string entropy)
    {
    }

    public static byte[] Encrypt(byte[] data, byte[] entropy)
    {
    }

    public static string Decrypt(string data, string entropy)
    {
    }

    public static string Decrypt(string data, byte[] entropy)
    {
    }

    public static byte[] Decrypt(byte[] data, string entropy)
    {
    }

    public static byte[] Decrypt(byte[] data, byte[] entropy)
    {
    }
}