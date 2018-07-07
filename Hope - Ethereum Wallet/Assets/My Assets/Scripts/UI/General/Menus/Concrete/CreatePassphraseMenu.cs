using NBitcoin;
using Nethereum.HdWallet;
using UnityEngine.UI;

/// <summary>
/// Class used for creating a new ethereum wallet.
/// </summary>
public class CreatePassphraseMenu : WalletLoadMenuBase<CreatePassphraseMenu>
{

    public Text mnemonicPhraseField;

    public Button confirmButton;
    public Button backButton;

    private string mnemonic;

    /// <summary>
    /// Initializes this class by creating a new mnemonic phrase and setting the text to it.
    /// </summary>
    private void Start()
    {
        confirmButton.onClick.AddListener(LoadWallet);
        backButton.onClick.AddListener(OnBackPressed);

        mnemonic = new Wallet(Wordlist.English, WordCount.Twelve).Phrase;
        mnemonicPhraseField.text = mnemonic;
    }

    /// <summary>
    /// Creates a wallet with the newly created mnemonic phrase.
    /// </summary>
    public override void LoadWallet() => userWalletManager.CreateWallet(mnemonic);

    /// <summary>
    /// Closes this menu when the back button is pressed.
    /// </summary>
    public override void OnBackPressed() => uiManager.CloseMenu();

}
