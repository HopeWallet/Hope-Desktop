using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;

/// <summary>
/// The animator class of the ConfirmMnemonicMenu
/// </summary>
public sealed class ConfirmMnemonicMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject backButton;
    [SerializeField] private GameObject wordInputField;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject[] checkBoxes;
	[SerializeField] private GameObject loadingIcon;
	[SerializeField] private Button hopeLogo;

	private DynamicDataCache dynamicDataCache;
    private ConfirmMnemonicMenu confirmMnemonicMenu;

    private int wordIndex;

	public bool OpeningWallet { get; private set; }

    /// <summary>
    /// Adds the DynamicDataCache dependency.
    /// </summary>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    [Inject]
    public void Construct(DynamicDataCache dynamicDataCache) => this.dynamicDataCache = dynamicDataCache;

    /// <summary>
    /// Initializes the necessary variables that haven't already been initialized in the inspector
    /// </summary>
    private void Awake()
    {
        confirmMnemonicMenu = GetComponent<ConfirmMnemonicMenu>();
		confirmMnemonicMenu.OnWalletLoading += CreateWallet;

		wordInputField.GetComponent<HopeInputField>().OnInputUpdated += _ => InputFieldChanged();
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
		wordInputField.GetComponent<HopeInputField>().InputFieldBase.ActivateInputField();
		wordInputField.AnimateScaleX(1f, 0.25f);
		nextButton.AnimateGraphicAndScale(1f, 1f, 0.3f);

		float duration = 0.325f;
		for (int i = 0; i < 4; i ++)
		{
			if (i != 3)
				checkBoxes[i].AnimateScale(1f, duration);
			else
				checkBoxes[i].AnimateScale(1f, duration, FinishedAnimating);

			duration += 0.025f;
		}
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		if (OpeningWallet)
			backButton.AnimateGraphicAndScale(0f, 0f, 0.3f);

		wordInputField.AnimateScale(0f, 0.3f);
		nextButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);

		for (int i = 0; i < 4; i++)
			checkBoxes[i].AnimateScale(0f, 0.3f);
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

		bool stringContainsNonLetter = inputField.Text.Any(c => !char.IsLetter(c));

		inputField.Error = string.IsNullOrEmpty(inputField.Text) || stringContainsNonLetter;

		if (!string.IsNullOrEmpty(inputField.Text))
			inputField.errorMessage.text = "Invalid word";

		nextButton.GetComponent<Button>().interactable = !inputField.Error;
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
				wordInputField.GetComponent<HopeInputField>().InputFieldBase.ActivateInputField();
			}
            else
            {
                OpeningWallet = true;
				inputField.InputFieldBase.interactable = false;
                checkBoxes[wordIndex].transform.GetChild(1).gameObject.AnimateGraphicAndScale(1f, 1f, 0.15f, confirmMnemonicMenu.LoadWallet);
				checkBoxes[wordIndex].transform.GetChild(0).gameObject.AnimateColor(UIColors.Green, 0.15f);
			}
        }
        else
        {
			inputField.Error = true;
			inputField.errorMessage.text = "Incorrect word";
			inputField.UpdateVisuals();
			nextButton.GetComponent<Button>().interactable = false;
		}
    }

	/// <summary>
	/// Animates the loading icon into view
	/// </summary>
	private void CreateWallet()
	{
		hopeLogo.interactable = false;
		hopeLogo.GetComponent<LoadingIconAnimator>().enabled = true;
		hopeLogo.GetComponent<MenuTooltipManager>().CloseMenuTooltip();

		Animating = true;
		nextButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
		loadingIcon.SetActive(true);
		loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.15f);
	}
}
