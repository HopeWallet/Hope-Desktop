using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu for importing an ethereum wallet.
/// </summary>
public class ImportWalletMenu : WalletLoaderBase<ImportWalletMenu>, IEnterButtonObservable
{

    public InputField seedInput;
    public Button importButton,
                  backButton;

    private ButtonClickObserver buttonObserver;

    private readonly int[] validWordCounts = new int[] { 12, 24 };

    /// <summary>
    /// Injects the required dependencies into this menu.
    /// </summary>
    /// <param name="buttonObserver"> The active ButtonObserver. </param>
    [Inject]
    public void Construct(ButtonClickObserver buttonObserver) => this.buttonObserver = buttonObserver;

    /// <summary>
    /// Adds the button click events.
    /// </summary>
    private void Start()
    {
        seedInput.onValueChanged.AddListener(val => CheckWordCount());
        importButton.onClick.AddListener(LoadWallet);
        backButton.onClick.AddListener(OnBackPressed);
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
    /// Checks the word count of the input field and sets the import button interactivity if the word count is valid.
    /// </summary>
    private void CheckWordCount()
    {
        var wordCount = seedInput.text.Split(' ', '\t', '\n').Length;
        importButton.interactable = validWordCounts.Where(count => count == wordCount).Count() > 0;
    }

    /// <summary>
    /// Loads the wallet if the button is enabled.
    /// </summary>
    /// <param name="clickType"> The enter button click type. </param>
    public void EnterButtonPressed(ClickType clickType)
    {
        if (InputFieldUtils.GetActiveInputField() == seedInput && importButton.interactable && clickType == ClickType.Down)
            importButton.Press();
    }

    /// <summary>
    /// Sends a message to the UserWalletManager to create a wallet with the InputField text as the mnemonic phrase.
    /// </summary>
    public override void LoadWallet() => userWalletManager.CreateWallet(seedInput.text);

    /// <summary>
    /// Closes this menu.
    /// </summary>
    public override void OnBackPressed() => uiManager.CloseMenu();
}
