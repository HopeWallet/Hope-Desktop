﻿using UnityEngine;
using TMPro;
using Hope.Utils.EthereumUtils;
using UnityEngine.UI;

public class ImportPassphraseForm : FormAnimation
{

	[SerializeField] private GameObject form1;
	[SerializeField] private GameObject backButton1;
	[SerializeField] private GameObject form2;
	[SerializeField] private GameObject backButton2;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject passphrase;
	[SerializeField] private GameObject wordCountDropdown;
	[SerializeField] private GameObject pastePhraseButton;
	[SerializeField] private GameObject importButton;
	[SerializeField] private GameObject checkMarkIcon;
	[SerializeField] private GameObject errorIcon;

	private GameObject[] wordInputField;
	private GameObject[] wordTextObjects;
	private TMP_Dropdown dropdownComponent;

	private string[] wordStrings; 

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	protected override void InitializeElements()
	{
		backButton1 = form1.transform.GetChild(0).gameObject;
		backButton2 = form2.transform.GetChild(0).gameObject;

		wordInputField = new GameObject[12];
		wordTextObjects = new GameObject[12];

		for (int i = 0; i < 12; i++)
		{
			wordInputField[i] = passphrase.transform.GetChild(i).gameObject;
			wordTextObjects[i] = wordInputField[i].transform.GetChild(0).GetChild(2).gameObject;

			wordInputField[i].GetComponent<TMP_InputField>().onValueChanged.AddListener((str) => SetButtonInteractable());
		}

		importButton.GetComponent<Button>().interactable = false;

		dropdownComponent = wordCountDropdown.GetComponent<TMP_Dropdown>();
		dropdownComponent.onValueChanged.AddListener(PassphraseWordCount);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		//STILL NEED TO CODE YA DICKHEAD!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		FinishedAnimatingIn();
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		//STILL NEED TO CODE YA DICKHEAD!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	}

	/// <summary>
	/// Splits the copied string on the clipboard into an array and sets them individually during the animations
	/// </summary>
	public void PastePhraseClicked()
	{
		string clipboard = ClipboardUtils.GetClipboardString();

		wordStrings = clipboard.GetMnemonicWords();

		if (wordStrings.Length == 12 && clipboard != "")
			StartWordAnimation();

		else
		{
			AnimateIcon(errorIcon);
			Debug.Log("Add popup saying clipboard text is not a passphrase");
		}
	}

	/// <summary>
	/// Starts the series of word animations
	/// </summary>
	private void StartWordAnimation()
	{
		AnimateIcon(checkMarkIcon);

		CrunchWord(0);
	}

	/// <summary>
	/// Scales the word's X value to zero
	/// </summary>
	/// <param name="index"> The index that is being animated </param>
	private void CrunchWord(int index)
	{
		wordTextObjects[index].AnimateScaleX(0f, 0.05f, () => ExpandWord(index));
	}

	/// <summary>
	/// Scales the word's X value back to 1
	/// </summary>
	/// <param name="index"> The index that is being animated </param>
	private void ExpandWord(int index)
	{
		wordInputField[index].GetComponent<TMP_InputField>().text = wordStrings[index];
		wordTextObjects[index].AnimateScaleX(1f, 0.05f, () => ProcessWordAnimation(++index));
	}

	/// <summary>
	/// If there are still words left to animate, it calls the CrunchWord animation again
	/// </summary>
	/// <param name="index"> The index that is being animated </param>
	private void ProcessWordAnimation(int index)
	{
		if (index < wordStrings.Length)
			CrunchWord(index);
	}

	/// <summary>
	/// Checks to see if all words are filled out and sets the import button to interactable if so
	/// </summary>
	private void SetButtonInteractable()
	{
		Button importButtonComponent = importButton.GetComponent<Button>();

		for (int i = 0; i < wordTextObjects.Length; i++)
		{
			if (wordInputField[i].GetComponent<TMP_InputField>().text == "")
			{
				importButtonComponent.interactable = false;
				return;
			}
		}

		importButtonComponent.interactable = true;
	}

	/// <summary>
	/// Animates an icon in and out of view
	/// </summary>
	/// <param name="gameObject"> The GameObject that is being animated </param>
	private void AnimateIcon(GameObject gameObject)
	{
		Animating = true;

		gameObject.transform.localScale = new Vector3(0, 0, 1);

		gameObject.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => gameObject.AnimateScaleX(1.01f, 1f,
			() => gameObject.AnimateGraphic(0f, 0.5f,
			() => Animating = false)));
	}

	/// <summary>
	/// Checks the value of the word count dropdown and adjusts the form accordingly
	/// </summary>
	/// <param name="value"> The value of the dropdown </param>
	private void PassphraseWordCount(int value)
	{
		if (value == 0)
			AnimateFormChange(false);

		else
			AnimateFormChange(true);
	}

	private void AnimateFormChange(bool bigForm)
	{
		form1.AnimateGraphic(0f, 0.2f);
		backButton1.AnimateGraphic(0f, 0.2f);
		form2.AnimateGraphic(1f, 0.2f);
		form2.AnimateScaleY(1f, 0.2f);
		backButton2.AnimateGraphic(1f, 0.2f);

		title.AnimateTransformY(283f, 0.2f);
		passphrase.AnimateTransformY(101f, 0.2f);
		wordCountDropdown.AnimateTransformY(308f, 0.2f);
		pastePhraseButton.AnimateTransformY(-236f, 0.2f);
		importButton.AnimateTransformY(-303f, 0.2f);
	}
}
