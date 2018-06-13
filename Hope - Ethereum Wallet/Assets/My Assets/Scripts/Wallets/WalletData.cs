using System;
using UnityEngine;

/// <summary>
/// Class containing ethereum wallet data.
/// </summary>
[Serializable]
public class WalletData
{

    /// <summary>
    /// The encrypted first half of the private key of the ethereum address.
    /// </summary>
    public string PrivateKey1 { get; set; }

    /// <summary>
    /// The encrypted second half of the private key of the ethereum address.
    /// </summary>
    public string PrivateKey2 { get; set; }

    /// <summary>
    /// The encrypted first half of the recovery phrase of the ethereum address.
    /// </summary>
    public string Phrase1 { get; set; }

    /// <summary>
    /// The encrypted second half of the recovery seed of the ethereum address.
    /// </summary>
    public string Phrase2 { get; set; }

    /// <summary>
    /// Initializes the ethereum wallet.
    /// </summary>
    /// <param name="pkey1"> The first half of the encrypted private key. </param>
    /// <param name="pkey2"> The second half of the encrypted private key. </param>
    /// <param name="phrase1"> The first half of the encrypted mnemonic phrase. </param>
    /// <param name="phrase2"> The second half of the encrypted mnemonic phrase. </param>
    public WalletData(string pkey1, string pkey2, string phrase1, string phrase2)
    {
        PrivateKey1 = pkey1;
        PrivateKey2 = pkey2;
        Phrase1 = phrase1;
        Phrase2 = phrase2;
    }

}
