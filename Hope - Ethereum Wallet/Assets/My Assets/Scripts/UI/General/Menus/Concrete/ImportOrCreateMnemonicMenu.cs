using UnityEngine.UI;

/// <summary>
/// Class which allows the user to choose whether to import or create a new wallet.
/// </summary>
public class ImportOrCreateMnemonicMenu : Menu<ImportOrCreateMnemonicMenu>
{

    public Button importButton;
    public Button createButton;
    public Button backButton;

    /// <summary>
    /// Adds the click events for the buttons of this menu.
    /// </summary>
    private void Start()
    {
        importButton.onClick.AddListener(ImportWallet);
        createButton.onClick.AddListener(CreateWallet);
        backButton.onClick.AddListener(GoBack);
    }

    /// <summary>
    /// Changes to the import wallet gui.
    /// </summary>
    public void ImportWallet() => uiManager.OpenMenu<ImportWalletMenu>();

    /// <summary>
    /// Changes to the create wallet gui.
    /// </summary>
    public void CreateWallet() => uiManager.OpenMenu<CreatePassphraseMenu>();

}
