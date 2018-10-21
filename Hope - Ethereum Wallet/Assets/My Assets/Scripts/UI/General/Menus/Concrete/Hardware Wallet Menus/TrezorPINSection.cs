using UnityEngine;
using UnityEngine.UI;

public sealed class TrezorPINSection : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button[] keypadButtons;
    [SerializeField] private HopeInputField passcodeInputField;

    private void Awake()
    {
        for (int i = 0; i < keypadButtons.Length; i++)
            AssignKeypadListener(i);

        //nextButton.onClick.AddListener()
    }

    private void AssignKeypadListener(int index)
    {

    }

    private void OnNextClicked()
    {

    }
}