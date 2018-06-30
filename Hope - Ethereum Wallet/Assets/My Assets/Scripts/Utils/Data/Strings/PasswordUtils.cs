using Nethereum.Signer;
using System;

/// <summary>
/// Class used for creating random password strings.
/// </summary>
public static class PasswordUtils
{

    /// <summary>
    /// Generates a random password of a given length.
    /// The characters of the password are all hexidecimal characters: 0-9, A-F.
    /// </summary>
    /// <param name="length"> The length of the password. </param>
    /// <returns> The new password. </returns>
    public static string GenerateFixedLengthPassword(int length = 12)
    {
        var encryptionPass = EthECKey.GenerateKey().GetPublicAddress();
        return encryptionPass.Remove(0, (encryptionPass.Length - length));
    }

    /// <summary>
    /// Generates a random password between random min and max bounds.
    /// </summary>
    /// <returns> The random password generated. </returns>
    public static string GenerateRandomPassword()
    {
        var random = new Random();
        return GenerateFixedLengthPassword(random.Next(random.Next(8, 12), random.Next(16, 24)));
    }
}
