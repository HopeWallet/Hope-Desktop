using System;

/// <summary>
/// Class which encrypts data which can be decrypted over several sessions of lifetime.
/// </summary>
public sealed class EternalEncryption
{
    private readonly bool isWindows;

    /// <summary>
    /// Initializes the <see cref="EternalEncryption"/> object.
    /// </summary>
    public EternalEncryption()
    {
        isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
    }

}