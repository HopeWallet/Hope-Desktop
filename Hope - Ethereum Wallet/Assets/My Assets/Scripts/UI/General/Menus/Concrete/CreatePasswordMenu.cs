using Hope.Security.Encryption;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// GUIMenu for creating a new password to be used with the new wallet.
/// </summary>
public class CreatePasswordMenu : Menu<CreatePasswordMenu>, ITabButtonObservable, IEnterButtonObservable
{

    public InputField[] passwordFields;

    public Button createPasswordButton;

    private UserWalletManager userWalletManager;
    private ButtonClickObserver buttonObserver;

    private const int PASSWORD_LENGTH = AESEncryption.MIN_PASSWORD_LENGTH;

    /// <summary>
    /// Injects the UserWalletManager as this class's dependency.
    /// </summary>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="buttonObserver"> The active ButtonObserver. </param>
    [Inject]
    public void Construct(UserWalletManager userWalletManager, ButtonClickObserver buttonObserver)
    {
        this.userWalletManager = userWalletManager;
        this.buttonObserver = buttonObserver;
    }

    /// <summary>
    /// Adds all the listener methods to the button and input fields.
    /// </summary>
    private void Start()
    {
        createPasswordButton.onClick.AddListener(SetPassword);
        passwordFields[0].onValueChanged.AddListener(val => UpdateButton());
        passwordFields[1].onValueChanged.AddListener(val => UpdateButton());
    }

    /// <summary>
    /// Subscribe this class to the button observer.
    /// </summary>
    private void OnEnable() => buttonObserver.SubscribeObservable(this);

    /// <summary>
    /// Unsubscribe this class from the button observer.
    /// </summary>
    private void OnDisable() => buttonObserver.UnsubscribeObservable(this);

    /// <summary>
    /// Updates the button's state based on the text in the input fields.
    /// </summary>
    public void UpdateButton() => createPasswordButton.interactable = CanAcceptPassword(passwordFields[0].text, passwordFields[1].text);

    /// <summary>
    /// Sets the user's password which will be used to encrypt the wallet shortly.
    /// </summary>
    public void SetPassword()
    {
        userWalletManager.SetWalletPassword(passwordFields[1].text);
        uiManager.OpenMenu<ImportOrCreateMenu>();
    }

    /// <summary>
    /// Checks if the password in the input fields can be accepted or not.
    /// </summary>
    /// <param name="firstPasswordInput"> The text in the first InputField. </param>
    /// <param name="secondPasswordInput"> The text in the second InputField. </param>
    /// <returns> Whether the password can be accepted or not. </returns>
    private bool CanAcceptPassword(string firstPasswordInput, string secondPasswordInput)
    {
        bool notEmpty = firstPasswordInput != "";
        bool isProperLength = firstPasswordInput.Length >= PASSWORD_LENGTH;
        bool equal = firstPasswordInput == secondPasswordInput;

        return notEmpty && isProperLength && equal;
    }

    /// <summary>
    /// Switch the password field on the enter button pressed.
    /// </summary>
    /// <param name="clickType"> The enter button click type. </param>
    public void EnterButtonPressed(ClickType clickType)
    {
        if (InputFieldUtils.GetActiveInputField() == passwordFields[0])
            SwitchField(clickType);
        else if (createPasswordButton.interactable && clickType == ClickType.Down)
            createPasswordButton.Press();
    }

    /// <summary>
    /// Switch the password field on the tab button pressed.
    /// </summary>
    /// <param name="clickType"> The tab button click type. </param>
    public void TabButtonPressed(ClickType clickType) => SwitchField(clickType);

    /// <summary>
    /// Switches the active password field.
    /// </summary>
    /// <param name="clickType"> The click type used to switch the field. </param>
    private void SwitchField(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        passwordFields.MoveToNextInputField();
    }

    public override void OnBackPressed()
    {
    }
}
