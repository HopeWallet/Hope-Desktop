using UnityEngine.UI;

public class ChooseWalletMenu : Menu<ChooseWalletMenu>
{

    public Button ledgerButton;
    public Button hopeButton;

    private void Start()
    {
        ledgerButton.onClick.AddListener(OpenLedgerWallet);
        hopeButton.onClick.AddListener(OpenHopeWallet);
    }

    private void OpenHopeWallet()
    {
        uiManager.OpenMenu<CreateWalletMenu>();
    }

    private void OpenLedgerWallet()
    {
    }

    public override void OnBackPressed()
    {
    }
}
