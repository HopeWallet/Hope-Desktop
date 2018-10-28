using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which manages and displays the area needed for entering the trezor pin.
/// </summary>
public sealed class TrezorPINSection : MonoBehaviour
{
    [SerializeField] private HopeInputField passcodeInputField;
    [SerializeField] private Button removeCharacterButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button[] keypadButtons;

    /// <summary>
    /// Button used to start checking if the pin entered is valid.
    /// </summary>
    public Button NextButton => nextButton;

    /// <summary>
    /// Input field used to enter the trezor pin.
    /// </summary>
    public HopeInputField PINInputField => passcodeInputField;

    /// <summary>
    /// The pin text.
    /// </summary>
    public string PinText { get; private set; } = string.Empty;

    /// <summary>
    /// Adds all the required events and button listeners.
    /// </summary>
    private void Awake()
    {
		passcodeInputField.OnInputUpdated += PinInputFieldChanged;

        for (int i = 0; i < keypadButtons.Length; i++)
            AssignKeypadListener(i);

        nextButton.onClick.AddListener(OnNextClicked);
        removeCharacterButton.onClick.AddListener(OnRemoveCharacterClicked);
    }

    /// <summary>
    /// Called when the pin input field changes.
    /// </summary>
    /// <param name="text"> The new pin text entered. </param>
	private void PinInputFieldChanged(string text)
	{
		passcodeInputField.Error = string.IsNullOrEmpty(text);
		nextButton.interactable = !passcodeInputField.Error;
	}

    /// <summary>
    /// Assigns the button listener for the corresponding keypad button.
    /// </summary>
    /// <param name="index"> The index of the keypad button to initialize. </param>
    private void AssignKeypadListener(int index)
    {
        keypadButtons[index].onClick.AddListener(() => OnKeypadButtonClicked(index));
    }

    /// <summary>
    /// Updates the text of the pin input field when a keypad button is clicked.
    /// </summary>
    /// <param name="index"> The index of the keypad button which was clicked. </param>
    private void OnKeypadButtonClicked(int index)
    {
        if (passcodeInputField.Text.Length != PinText.Length)
            PinText = string.Empty;

        passcodeInputField.Error = false;
        passcodeInputField.Text += (index + 1).ToString();
        PinText += (index + 1).ToString();

        if (!nextButton.interactable)
            nextButton.interactable = true;
    }

    /// <summary>
    /// Sets the next button to non-interactable once it is clicked.
    /// </summary>
    private void OnNextClicked()
    {
        nextButton.interactable = false;
    }

    /// <summary>
    /// Removes a character from the pin input field when the backspace button is clicked.
    /// </summary>
    private void OnRemoveCharacterClicked()
    {
        passcodeInputField.Text = passcodeInputField.Text.LimitEnd(passcodeInputField.Text.Length - 1);
        PinText = passcodeInputField.Text.Length == 0 ? string.Empty : PinText.LimitEnd(passcodeInputField.Text.Length);

        if (PinText.Length == 0)
            nextButton.interactable = false;
    }
}