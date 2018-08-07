using System;

[Serializable]
public sealed class WalletInfoJson
{
    public string walletName;
    public string[] walletAddresses;

    public WalletInfoJson(string walletName, string[] walletAddresses)
    {
        this.walletName = walletName;
        this.walletAddresses = walletAddresses;
    }
}