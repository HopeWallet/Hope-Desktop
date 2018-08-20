using UnityEngine;
using Hope.Utils.Ethereum;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using NBitcoin;

/// <summary>
/// Class used for animating the ImportMnemonicMenu.
/// </summary>
public class ImportMnemonicMenuAnimator : UIAnimator
{
	[SerializeField] private GameObject passphrase;
	[SerializeField] private GameObject wordCountSection;
	[SerializeField] private GameObject pastePhraseButton;
	[SerializeField] private GameObject nextButton;
	[SerializeField] private GameObject checkMarkIcon;
	[SerializeField] private GameObject errorIcon;

	private HopeInputField[] wordInputFields;
	private GameObject[] wordTextObjects;

	private ImportMnemonicMenu importMnemonicMenu;

	private string[] wordStrings;
	private int wordCount = 12;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		importMnemonicMenu = transform.GetComponent<ImportMnemonicMenu>();

		wordInputFields = new HopeInputField[24];
		wordTextObjects = new GameObject[24];

		for (int i = 0; i < wordInputFields.Length; i++)
			SetInputFieldVariables(i);

		wordCountSection.GetComponent<RadioButtons>().OnButtonChanged += PassphraseWordCountChanged;
		pastePhraseButton.GetComponent<Button>().onClick.AddListener(PastePhraseClicked);
		importMnemonicMenu.LastSelectableField = wordInputFields[11].inputFieldBase;
	}

	/// <summary>
	/// Sets the input field variables at a given index
	/// </summary>
	/// <param name="i"> The index of the input field in the hiearchy </param>
	private void SetInputFieldVariables(int i)
	{
		wordInputFields[i] = passphrase.transform.GetChild(i).GetComponent<HopeInputField>();
		wordTextObjects[i] = wordInputFields[i].transform.GetChild(1).GetChild(0).gameObject;

		wordInputFields[i].OnInputUpdated += () => CheckIfValidInput(i);

		if (wordInputFields[i].inputFieldBase.interactable)
			importMnemonicMenu.SelectableFields.Add(wordInputFields[i].inputFieldBase);
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		FinishedAnimating();
	}

	/// <summary>
	/// Resets the unique elements of the form back to the starting positions
	/// </summary>
	protected override void ResetElementValues()
	{
		FinishedAnimating();
	}

	/// <summary>
	/// Starts the series of word animations
	/// </summary>
	private void StartWordAnimation()
	{
		wordInputFields.ForEach(i => i.Text = string.Empty);
		wordTextObjects.ForEach(t => t.transform.localScale = new Vector2(0f, 1f));

		Animating = true;
		AnimateWord(0);
	}

	/// <summary>
	/// Animates the word object and sets the input field to the pasted word
	/// </summary>
	/// <param name="index"> The index of the current input field </param>
	private void AnimateWord(int index)
	{
		wordInputFields[index].Text = wordStrings[index];
		wordTextObjects[index].AnimateScaleX(1f, 0.1f, () => ProcessWordAnimation(++index));
	}

	/// <summary>
	/// If there are still words left to animate, it calls the CrunchWord animation again
	/// </summary>
	/// <param name="index"> The index of the current input field </param>
	private void ProcessWordAnimation(int index)
	{
		if (index < wordStrings.Length)
		{
			AnimateWord(index);
		}

		else
		{
			Animating = false;
			SetButtonInteractable();
		}
	}

	/// <summary>
	/// Animates an icon in and out of view
	/// </summary>
	/// <param name="gameObject"> The GameObject that is being animated </param>
	private void AnimateIcon(GameObject gameObject)
	{
		gameObject.transform.localScale = new Vector3(0, 0, 1);

		gameObject.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => CoroutineUtils.ExecuteAfterWait(0.6f, () => gameObject.AnimateGraphic(0f, 0.25f)));
	}

	/// <summary>
	/// Checks the value of currently selected word count
	/// </summary>
	/// <param name="value"> The value of the dropdown </param>
	private void PassphraseWordCountChanged(WordCount value)
	{
		wordCount = (int)value;

		SetInteractableFields();
		SetButtonInteractable();
	}

	/// <summary>
	/// Sets the input field visuals depending on if they are interactable or not, and sets the selectable input fields
	/// </summary>
	private void SetInteractableFields()
	{
		importMnemonicMenu.SelectableFields.Clear();

		for (int i = 0; i < 24; i++)
		{
			bool interactable = i < wordCount;
			InputField baseInputField = wordInputFields[i].inputFieldBase;

			if (interactable)
			{
				importMnemonicMenu.LastSelectableField = baseInputField;
				importMnemonicMenu.SelectableFields.Add(baseInputField);
			}
			else
			{
				wordInputFields[i].Text = string.Empty;
			}

			if (string.IsNullOrEmpty(wordInputFields[i].Text))
				baseInputField.gameObject.AnimateColor(interactable ? UIColors.White : new Color(0.45f, 0.45f, 0.45f), 0.15f);

			baseInputField.interactable = interactable;
			wordInputFields[i].transform.GetChild(0).gameObject.AnimateColor(interactable ? UIColors.White : UIColors.DarkGrey, 0.15f);
		}
	}

	/// <summary>
	/// Checks if the text in the input field is a valid input
	/// </summary>
	/// <param name="index"> The index of the input field in the hiearchy </param>
	private void CheckIfValidInput(int index)
	{
		wordInputFields[index].Error = string.IsNullOrEmpty(wordInputFields[index].Text) || wordInputFields[index].Text.Any(char.IsDigit);
		SetButtonInteractable();
	}

	/// <summary>
	/// Checks to see if all words are filled out and sets the import button to interactable if so
	/// </summary>
	private void SetButtonInteractable()
	{
		if (Animating)
			return;

		var compatibleWords = wordInputFields.Where(b => b.inputFieldBase.interactable && !b.Error).ToList();

		nextButton.GetComponent<Button>().interactable = compatibleWords.Count == wordCount;
	}

	/// <summary>
	/// Splits the copied string on the clipboard into an array and sets them individually during the animations
	/// </summary>
	private void PastePhraseClicked()
	{
		string clipboard = ClipboardUtils.GetClipboardString();
		wordStrings = WalletUtils.GetMnemonicWords(clipboard);

		int numOfWords = wordStrings.Length;

		bool emptyClipboard = string.IsNullOrEmpty(clipboard);

		if (!emptyClipboard && numOfWords <= 24)
		{
			wordCountSection.GetComponent<RadioButtons>().RadioButtonClicked(numOfWords <= 12 ? 0 : numOfWords <= 15 ? 1 : numOfWords <= 18 ? 2 : numOfWords <= 21 ? 3 : 4);

			AnimateIcon(checkMarkIcon);
			StartWordAnimation();
		}

		else
		{
			AnimateIcon(errorIcon);

			//Add error messages beside the error icon

			//if (emptyClipboard)
			//Error message says: "Clipboard empty."

			//else
			//Error message says: "Too many words."
		}
	}
}
