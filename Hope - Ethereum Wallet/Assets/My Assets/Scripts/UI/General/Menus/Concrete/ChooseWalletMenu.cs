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
        uiManager.OpenMenu<CreateWalletMenu>();
    }

    /// <summary>
    /// Opens the Ledger wallet.
    /// </summary>
    private void OpenLedgerWallet()
    {
    }

    public override void OnBackPressed()
    {
    }
}
