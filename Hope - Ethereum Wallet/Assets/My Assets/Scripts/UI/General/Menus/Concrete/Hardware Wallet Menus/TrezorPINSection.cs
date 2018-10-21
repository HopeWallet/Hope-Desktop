using UnityEngine;
using UnityEngine.UI;

public sealed class TrezorPINSection : MonoBehaviour
{
    [SerializeField] private HopeInputField passcodeInputField;
    [SerializeField] private Button removeCharacterButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button[] keypadButtons;

    public Button NextButton => nextButton;

    public string PinText => passcodeInputField.InputFieldBytes.GetUTF8String();

    private void Awake()
    {
        for (int i = 0; i < keypadButtons.Length; i++)
            AssignKeypadListener(i);

        nextButton.onClick.AddListener(OnNextClicked);
        removeCharacterButton.onClick.AddListener(OnRemoveCharacterClicked);
    }

    private void AssignKeypadListener(int index)
    {
        keypadButtons[index].onClick.AddListener(() => OnKeypadButtonClicked(index));
    }

    private void OnKeypadButtonClicked(int index)
    {
        passcodeInputField.Text = passcodeInputField.InputFieldBytes.GetUTF8String() + (index + 1).ToString();
    }

    private void OnNextClicked()
    {
        passcodeInputField.Text = string.Empty;
    }

    private void OnRemoveCharacterClicked()
    {
        passcodeInputField.Text = passcodeInputField.InputFieldBytes.GetUTF8String().LimitEnd(passcodeInputField.InputFieldBytes.Length - 1);
    }
}