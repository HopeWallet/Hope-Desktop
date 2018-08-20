using UnityEngine;
using Hope.Utils.Ethereum;
using UnityEngine.UI;
using System.Linq;
using TMPro;

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

	private string[] wordStrings;
	private int wordCount = 12;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		wordInputFields = new HopeInputField[24];
		wordTextObjects = new GameObject[24];

		for (int i = 0; i < wordInputFields.Length; i++)
			SetInputFields(i);

		wordCountSection.GetComponent<RadioButtons>().OnButtonChanged += PassphraseWordCountChanged;
		pastePhraseButton.GetComponent<Button>().onClick.AddListener(PastePhraseClicked);
	}

	private void SetInputFields(int i)
	{
		wordInputFields[i] = passphrase.transform.GetChild(i).GetComponent<HopeInputField>();
		wordTextObjects[i] = wordInputFields[i].transform.GetChild(1).GetChild(0).gameObject;

		wordInputFields[i].OnInputUpdated += () => CheckIfValidInput(i);
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
		for (int i = 0; i < 24; i++)
		{
			wordInputFields[i].Text = string.Empty;
			wordTextObjects[i].transform.localScale = new Vector2(0f, 1f);
		}

		Animating = true;
		AnimateWord(0, 0);
	}

	/// <summary>
	/// Animates the word object and sets the input field to the pasted word
	/// </summary>
	/// <param name="wordIndex"> The index of the word in the wordStrings array </param>
	/// <param name="fieldIndex"> The index of the wordInputFields array </param>
	private void AnimateWord(int wordIndex, int fieldIndex)
	{
		if (!wordInputFields[fieldIndex].gameObject.activeInHierarchy)
		{
			ProcessWordAnimation(wordIndex, fieldIndex + 3);
			return;
		}

		wordInputFields[fieldIndex].Text = wordStrings[wordIndex];
		wordTextObjects[fieldIndex].AnimateScaleX(1f, 0.1f, () => ProcessWordAnimation(++wordIndex, ++fieldIndex));
	}

	/// <summary>
	/// If there are still words left to animate, it calls the CrunchWord animation again
	/// </summary>
	/// <param name="wordIndex"> The index of the word in the wordStrings array </param>
	/// <param name="fieldIndex"> The index of the wordInputFields array </param>
	private void ProcessWordAnimation(int wordIndex, int fieldIndex)
	{
		if (wordIndex < wordStrings.Length)
		{
			AnimateWord(wordIndex, fieldIndex);
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
	private void PassphraseWordCountChanged(RadioButtons.WordCount value)
	{
		wordCount = value == RadioButtons.WordCount.TwelveWords ? 12 : 24;

		passphrase.AnimateTransformX(wordCount == 12 ? 300f : 0f, 0.1f);
		//ChangeColumn(wordCount == 12, 3, 9, 15, 21);

		SetButtonInteractable();
	}

	//private void ChangeColumn(bool twelveWords, params int[] nums)
	//{
	//	for (int i = 0; i < nums.Length; i++)
	//	{
	//		wordInputFields[nums[i] - 3].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = wordInputFields[twelveWords ? nums[i] + 3 : nums[i] - 3].name;

	//		if (!twelveWords)
	//			wordInputFields[nums[i]].gameObject.SetActive(true);

	//		wordInputFields[nums[i]].gameObject.AnimateScaleX(twelveWords ? 0f : 1f, 0.15f, () => { if (twelveWords) wordInputFields[nums[i]].gameObject.SetActive(false); });
	//	}

	//	if (nums[3] != 23)
	//		ChangeColumn(twelveWords, ++nums[0], ++nums[1], ++nums[2], ++nums[3]);
	//}

	private void CheckIfValidInput(int index)
	{
		wordInputFields[index].Error = string.IsNullOrEmpty(wordInputFields[index].Text.Trim());

		SetButtonInteractable();
	}

	/// <summary>
	/// Checks to see if all words are filled out and sets the import button to interactable if so
	/// </summary>
	private void SetButtonInteractable()
	{
		if (Animating)
			return;

		var compatibleWords = wordInputFields.Where(b => b.isActiveAndEnabled && !b.Error).ToList();

		nextButton.GetComponent<Button>().interactable = compatibleWords.Count == wordCount;
	}

	/// <summary>
	/// Splits the copied string on the clipboard into an array and sets them individually during the animations
	/// </summary>
	private void PastePhraseClicked()
	{
		string clipboard = ClipboardUtils.GetClipboardString();
		wordStrings = WalletUtils.GetMnemonicWords(clipboard);

		int words = wordStrings.Length;

		if (clipboard != null && words <= 24)
		{
			wordCountSection.GetComponent<RadioButtons>().RadioButtonClicked(words <= 12 ? 0 : 1);

			AnimateIcon(checkMarkIcon);
			StartWordAnimation();
		}

		else
		{
			AnimateIcon(errorIcon);
		}
	}
}
