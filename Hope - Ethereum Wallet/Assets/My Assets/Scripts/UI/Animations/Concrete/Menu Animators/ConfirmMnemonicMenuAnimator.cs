﻿using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;

/// <summary>
/// The animator class of the ConfirmMnemonicMenu
/// </summary>
public sealed class ConfirmMnemonicMenuAnimator : MenuAnimator
{
    [SerializeField] private GameObject wordInputField;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject[] checkBoxes;
	[SerializeField] private GameObject loadingIcon;

	private DynamicDataCache dynamicDataCache;
    private ConfirmMnemonicMenu confirmMnemonicMenu;

    private int wordIndex;

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
	protected override void AnimateIn()
	{
		base.AnimateIn();

		FinishedAnimating();
	}

	/// <summary>
	/// Animates the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		FinishedAnimating();
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
			inputField.errorMessage.text = "Incorrect word";
			inputField.UpdateVisuals();
			nextButton.GetComponent<Button>().interactable = false;
		}
    }

	private void CreateWallet()
	{
		nextButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
		loadingIcon.SetActive(true);
		loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.15f);
	}
}
