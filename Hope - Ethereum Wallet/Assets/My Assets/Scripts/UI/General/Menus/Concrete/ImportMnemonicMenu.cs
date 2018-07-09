using System.Linq;
using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu for importing an ethereum wallet.
/// </summary>
public class ImportMnemonicMenu : WalletLoadMenuBase<ImportMnemonicMenu>, IEnterButtonObservable
{
    public Button importButton,
                  backButton1,
                  backButton2;

    public TMP_InputField[] wordFields;

    private ButtonClickObserver buttonObserver;
    private DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Injects the required dependencies into this menu.
    /// </summary>
    /// <param name="buttonObserver"> The active ButtonObserver. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache </param>
    [Inject]
    public void Construct(ButtonClickObserver buttonObserver, DynamicDataCache dynamicDataCache)
    {
        this.buttonObserver = buttonObserver;
        this.dynamicDataCache = dynamicDataCache;
    }

    /// <summary>
    /// Adds the button click events.
    /// </summary>
    private void Start()
    {
        importButton.onClick.AddListener(LoadWallet);
        backButton1.onClick.AddListener(GoBack);
        backButton2.onClick.AddListener(GoBack);
    }

    /// <summary>
    /// Subscribes this IEnterButtonObserver.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        buttonObserver.SubscribeObservable(this);
    }

    /// <summary>
    /// Unsubscribes this IEnterButtonObserver.
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        buttonObserver.UnsubscribeObservable(this);
    }

    /// <summary>
    /// Loads the wallet if the button is enabled.
    /// </summary>
    /// <param name="clickType"> The enter button click type. </param>
    public void EnterButtonPressed(ClickType clickType)
    {
        if (wordFields.Contains(InputFieldUtils.GetActiveTMPInputField()) && importButton.interactable && clickType == ClickType.Down)
            importButton.Press();
    }

    /// <summary>
    /// Sends a message to the UserWalletManager to create a wallet with the InputField text as the mnemonic phrase.
    /// </summary>
    public override void LoadWallet()
    {
        dynamicDataCache.SetData("mnemonic", string.Join(" ", wordFields.Select(field => field.text)));
        userWalletManager.CreateWallet(dynamicDataCache.GetData("mnemonic"));
    }
}
