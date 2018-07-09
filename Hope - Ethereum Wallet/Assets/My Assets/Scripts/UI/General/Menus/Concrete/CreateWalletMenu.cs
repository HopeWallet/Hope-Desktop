using Hope.Security.ProtectedTypes.Types;
using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu which lets the user create a new wallet by first choosing a password and name for the wallet.
/// </summary>
public sealed class CreateWalletMenu : Menu<CreateWalletMenu>
{

    public Button createWalletButton;
    public Button backButton;
    public TMP_InputField walletNameField;
    public TMP_InputField passwordField;

    private DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Adds the required dependencies into this class.
    /// </summary>
    /// <param name="dynamicDataCache"> The active ProtectedStringDataCache. </param>
    [Inject]
    public void Construct(DynamicDataCache dynamicDataCache) => this.dynamicDataCache = dynamicDataCache;

    /// <summary>
    /// Adds the button listeners.
    /// </summary>
    private void Start()
    {
        createWalletButton.onClick.AddListener(CreateWalletNameAndPass);
        backButton.onClick.AddListener(GoBack);
    }

    /// <summary>
    /// Sets up the wallet name and password and opens the next menu.
    /// </summary>
    private void CreateWalletNameAndPass()
    {
        dynamicDataCache.SetData("pass", new ProtectedString(passwordField.text));
        dynamicDataCache.SetData("name", walletNameField.text);

        uiManager.OpenMenu<ImportOrCreateMnemonicMenu>();
    }
}