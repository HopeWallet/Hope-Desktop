using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ConfirmMnemonicMenuAnimator : UIAnimator
{
    [SerializeField] private GameObject wordInputField;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject[] checkBoxes;

    private DynamicDataCache dynamicDataCache;
    private ConfirmMnemonicMenu confirmMnemonicMenu;

    private int wordIndex;
    private bool errorIconVisible;

    /// <summary>
    /// Adds the DynamicDataCache dependency.
    /// </summary>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    [Inject]
    public void Construct(DynamicDataCache dynamicDataCache) => this.dynamicDataCache = dynamicDataCache;

    /// <summary>
    /// Makes button interactable if the errorIcon is set to visible
    /// </summary>
    private bool ErrorIconVisible
    {
        set
        {
            errorIconVisible = value;
            if (errorIconVisible) nextButton.GetComponent<Button>().interactable = false;
        }
    }

    /// <summary>
    /// Initializes the necessary variables that haven't already been initialized in the inspector
    /// </summary>
    private void Awake()
    {
        confirmMnemonicMenu = GetComponent<ConfirmMnemonicMenu>();

        wordInputField.GetComponent<HopeInputField>().OnInputUpdated += InputFieldChanged;
        nextButton.GetComponent<Button>().onClick.AddListener(NextButtonClicked);
    }

    /// <summary>
    /// Sets the initial word text to what the first word should be
    /// </summary>
    private void Start() => SetWordText();

    /// <summary>
    /// Animates the unique elements of this form into view
    /// </summary>
    protected override void AnimateUniqueElementsIn()
    {
		FinishedAnimating();
        //wordInputField.AnimateScaleX(1f, 0.2f);
        //nextButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
        //AnimateCheckboxes(0);
    }

    /// <summary>
    /// Resets the unique elements of the form back to the starting positions
    /// </summary>
    protected override void ResetElementValues()
    {
        FinishedAnimating();

        //for (int i = 0; i < 4; i++)
        //    checkBoxes[i].SetScale(Vector2.zero);

        //wordInputField.SetScale(new Vector2(0f, 1f));
        //nextButton.SetGraphicAndScale(Vector2.zero);
    }

    /// <summary>
    /// Animates the checkboxes one by one
    /// </summary>
    /// <param name="index"> The index of the checkboxes array being animated </param>
    private void AnimateCheckboxes(int index)
    {
        if (index != 3)
            checkBoxes[index].AnimateScale(1f, 0.075f, () => AnimateCheckboxes(++index));
        else
            checkBoxes[index].AnimateScale(1f, 0.075f, FinishedAnimating);
    }

    /// <summary>
    /// Sets the various text to ask for the next number
    /// </summary>
    private void SetWordText()
    {
        int[] randomNums = dynamicDataCache.GetData("confirmation numbers");

		HopeInputField inputField = wordInputField.GetComponent<HopeInputField>();
		inputField.SetPlaceholderText("Word #" + randomNums[wordIndex]);
		inputField.Text = string.Empty;
    }

	/// <summary>
	/// Sets the next button to interactable if the word input field has at least something in the input
	/// </summary>
	/// <param name="str"> The current string that is in the word input field </param>
	private void InputFieldChanged()
	{
		HopeInputField inputField = wordInputField.GetComponent<HopeInputField>();

		nextButton.GetComponent<Button>().interactable = !string.IsNullOrEmpty(inputField.Text);

		if (inputField.Error && !string.IsNullOrEmpty(inputField.Text))
			inputField.Error = false;
	}

	/// <summary>
	/// The nextButton has been clicked
	/// </summary>
	private void NextButtonClicked()
    {
        var word = ((string[])dynamicDataCache.GetData("confirmation words"))[wordIndex];
		HopeInputField inputField = wordInputField.GetComponent<HopeInputField>();

		if (inputField.Text.EqualsIgnoreCase(word))
        {
            if (wordIndex != checkBoxes.Length - 1)
            {
                checkBoxes[wordIndex].transform.GetChild(1).gameObject.AnimateGraphicAndScale(1f, 1f, 0.15f);
				checkBoxes[wordIndex].transform.GetChild(0).gameObject.AnimateColor(UIColors.Green, 0.15f);
                ++wordIndex;
				SetWordText();
			}
            else
            {
				nextButton.GetComponent<InteractableButton>().OnCustomPointerExit();
                checkBoxes[wordIndex].transform.GetChild(1).gameObject.AnimateGraphicAndScale(1f, 1f, 0.15f, confirmMnemonicMenu.LoadWallet);
				checkBoxes[wordIndex].transform.GetChild(0).gameObject.AnimateColor(UIColors.Green, 0.15f);
			}
        }
        else
        {
			inputField.Error = true;
			inputField.UpdateVisuals(false);
			nextButton.GetComponent<Button>().interactable = false;
		}
    }
}
