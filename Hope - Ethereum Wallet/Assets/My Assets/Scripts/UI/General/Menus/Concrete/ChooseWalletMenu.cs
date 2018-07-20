using UnityEngine.UI;

/// <summary>
/// Menu which allows the user to choose to open a wallet via Ledger or the built in Hope wallet.
/// </summary>
public sealed class ChooseWalletMenu : Menu<ChooseWalletMenu>
{
    public Button ledgerButton;
    public Button hopeButton;
    public Button exitButton;

    /// <summary>
    /// Adds the button listeners on start.
    /// </summary>
    private void Start()
    {
        ledgerButton.onClick.AddListener(OpenLedgerWallet);
        hopeButton.onClick.AddListener(OpenHopeWallet);
    }

    /// <summary>
    /// Opens the Hope wallet.
    /// </summary>
    private void OpenHopeWallet()
    {
        HOPETesting.dataEncrypt.Decrypt(HOPETesting.encryptedString);

        if (SecurePlayerPrefs.HasKey("wallet_count") && SecurePlayerPrefs.GetInt("wallet_count") > 0)
            uiManager.OpenMenu<WalletListMenu>();
        else
            uiManager.OpenMenu<CreateWalletMenu>();
    }

    /// <summary>
    /// Opens the Ledger wallet.
    /// </summary>
    [SecureCallEnd]
    private void OpenLedgerWallet()
    {
        // TODO
        HOPETesting.dataEncrypt.Decrypt(HOPETesting.encryptedString);
    }

    /// <summary>
    /// Opens the ExitConfirmationPopup which allows the user to exit the wallet.
    /// </summary>
    protected override void OnBackPressed()
    {
        // TODO
    }
}
