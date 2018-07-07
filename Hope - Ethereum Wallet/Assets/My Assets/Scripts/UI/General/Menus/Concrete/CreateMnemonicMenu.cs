using NBitcoin;
using Nethereum.HdWallet;
using UnityEngine.UI;

/// <summary>
/// Class used for creating a new ethereum wallet.
/// </summary>
public class CreateMnemonicMenu : WalletLoadMenuBase<CreateMnemonicMenu>
{

    public Button confirmButton;
    public Button backButton;

    private string mnemonic;

    /// <summary>
    /// Initializes this class by creating a new mnemonic phrase and setting the text to it.
    /// </summary>
    private void Start()
    {
        confirmButton.onClick.AddListener(LoadWallet);
        backButton.onClick.AddListener(GoBack);

        mnemonic = new Wallet(Wordlist.English, WordCount.Twelve).Phrase;
    }

    /// <summary>
    /// Creates a wallet with the newly created mnemonic phrase.
    /// </summary>
    public override void LoadWallet() => userWalletManager.CreateWallet(mnemonic);

}
