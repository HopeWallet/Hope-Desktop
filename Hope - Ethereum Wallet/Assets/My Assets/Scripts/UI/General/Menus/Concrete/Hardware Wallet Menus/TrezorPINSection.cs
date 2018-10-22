using UnityEngine;
using UnityEngine.UI;

public sealed class TrezorPINSection : MonoBehaviour
{
    [SerializeField] private HopeInputField passcodeInputField;
    [SerializeField] private Button removeCharacterButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button[] keypadButtons;

    public Button NextButton => nextButton;

    public HopeInputField PINInputField => passcodeInputField;

    public string PinText { get; private set; } = string.Empty;

    private void Awake()
    {
		passcodeInputField.OnInputUpdated += PasscodeInputFieldChanged;

        for (int i = 0; i < keypadButtons.Length; i++)
            AssignKeypadListener(i);

        nextButton.onClick.AddListener(OnNextClicked);
        removeCharacterButton.onClick.AddListener(OnRemoveCharacterClicked);
    }
	
	private void PasscodeInputFieldChanged(string text)
	{
		passcodeInputField.Error = string.IsNullOrEmpty(text);
		nextButton.interactable = !passcodeInputField.Error;
	}

    private void AssignKeypadListener(int index)
    {
        keypadButtons[index].onClick.AddListener(() => OnKeypadButtonClicked(index));
    }

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

    private void OnNextClicked()
    {
        nextButton.interactable = false;
    }

    private void OnRemoveCharacterClicked()
    {
        passcodeInputField.Text = passcodeInputField.Text.LimitEnd(passcodeInputField.Text.Length - 1);
        PinText = passcodeInputField.Text.Length == 0 ? string.Empty : PinText.LimitEnd(passcodeInputField.Text.Length);

        if (PinText.Length == 0)
            nextButton.interactable = false;
    }
}