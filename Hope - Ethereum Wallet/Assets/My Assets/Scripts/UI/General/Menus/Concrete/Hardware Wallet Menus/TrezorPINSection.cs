using UnityEngine;
using UnityEngine.UI;

public sealed class TrezorPINSection : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button[] keypadButtons;
    [SerializeField] private HopeInputField passcodeInputField;

    public Button NextButton => nextButton;

    public string PinText => passcodeInputField.InputFieldBytes.GetUTF8String();

    private void Awake()
    {
        for (int i = 0; i < keypadButtons.Length; i++)
            AssignKeypadListener(i);
    }

    private void AssignKeypadListener(int index)
    {

    }

    private void OnNextClicked()
    {

    }
}