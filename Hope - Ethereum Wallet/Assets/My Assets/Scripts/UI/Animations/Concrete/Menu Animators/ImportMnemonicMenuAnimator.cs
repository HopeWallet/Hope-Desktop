using UnityEngine;
using TMPro;
using Hope.Utils.EthereumUtils;
using UnityEngine.UI;

/// <summary>
/// Class used for animating the ImportMnemonicMenu.
/// </summary>
public class ImportMnemonicMenuAnimator : UIAnimator
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

	private GameObject[] wordInputFields;
	private GameObject[] wordTextObjects;
	private TMP_Dropdown dropdownComponent;

	private string[] wordStrings;
	private int wordCount = 12;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		backButton1 = form1.transform.GetChild(0).gameObject;
		backButton2 = form2.transform.GetChild(0).gameObject;

		wordInputFields = new GameObject[24];
		wordTextObjects = new GameObject[24];

		for (int i = 0; i < wordInputFields.Length; i++)
		{
			wordInputFields[i] = passphrase.transform.GetChild(i).gameObject;
			wordTextObjects[i] = wordInputFields[i].transform.GetChild(0).GetChild(1).gameObject;

			wordInputFields[i].GetComponent<TMP_InputField>().onValueChanged.AddListener(_ => SetButtonInteractable());
		}

		dropdownComponent = wordCountDropdown.GetComponent<TMP_Dropdown>();
		dropdownComponent.onValueChanged.AddListener(PassphraseWordCountChanged);
		pastePhraseButton.GetComponent<Button>().onClick.AddListener(PastePhraseClicked);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		form1.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
			() => wordCountDropdown.AnimateGraphicAndScale(1f, 1f, 0.2f, FinishedAnimating)));

		pastePhraseButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => importButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
            () => AnimateRow(0, true, 12)));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		wordCountDropdown.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f, AnimateFormsOut));

		pastePhraseButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => importButton.AnimateGraphicAndScale(0f, 0f, 0.15f));

		for (int i = 0; i < wordCount; i++)
			wordInputFields[i].AnimateScaleX(0f, 0.15f);
	}

	/// <summary>
	/// Animates both forms off the screen
	/// </summary>
	private void AnimateFormsOut()
	{
		form1.AnimateGraphicAndScale(0f, 0f, 0.15f);
		form2.AnimateScaleY(0f, 0.15f, FinishedAnimating);

		backButton1.AnimateGraphicAndScale(0f, 0f, 0.15f);
		backButton2.AnimateGraphicAndScale(0f, 0f, 0.15f);
	}

	/// <summary>
	/// Starts the series of word animations
	/// </summary>
	private void StartWordAnimation()
	{
		Animating = true;
		AnimateIcon(checkMarkIcon);
		AnimateWord(0);
	}

	/// <summary>
	/// Animates the word object and sets the input field to the pasted word
	/// </summary>
	/// <param name="index"> The index that is being animated </param>
	private void AnimateWord(int index)
	{
		wordTextObjects[index].AnimateScaleX(0f, 0.05f, () =>
		{
			wordInputFields[index].GetComponent<TMP_InputField>().text = wordStrings[index];
			wordTextObjects[index].AnimateScaleX(1f, 0.05f, () => ProcessWordAnimation(++index));
		});
	}

	/// <summary>
	/// If there are still words left to animate, it calls the CrunchWord animation again
	/// </summary>
	/// <param name="index"> The index that is being animated </param>
	private void ProcessWordAnimation(int index)
	{
		if (index < wordStrings.Length && !string.IsNullOrEmpty(wordStrings[index]))
		{
			AnimateWord(index);
		}

		else
		{
			for (int i = index; i < 24; i++)
				wordInputFields[i].GetComponent<TMP_InputField>().text = "";

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

        gameObject.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => gameObject.AnimateScaleX(1.01f, 1f,
			() => gameObject.AnimateGraphic(0f, 0.5f)));
	}

	/// <summary>
	/// Checks the value of the word count dropdown and adjusts the form accordingly
	/// </summary>
	/// <param name="value"> The value of the dropdown </param>
	private void PassphraseWordCountChanged(int value)
	{
		wordCount = value == 0 ? 12 : 24;
		AnimateFormChange(wordCount == 24);

		SetButtonInteractable();
	}

	/// <summary>
	/// Changes the form to either accept a 12 word passphrase or 24 word passphrase
	/// </summary>
	/// <param name="bigForm"> Boolean that checks if it is a 24 word passphrase, animating this form to the bigger version </param>
	private void AnimateFormChange(bool bigForm)
	{
		form1.AnimateGraphic(bigForm ? 0f : 1f, 0.2f);
		backButton1.AnimateGraphic(bigForm ? 0f : 1f, 0.2f);
		form2.AnimateGraphic(bigForm ? 1f : 0f, 0.2f);
		form2.AnimateScaleY(bigForm ? 1f : 0f, 0.2f);
		backButton2.AnimateGraphicAndScale(bigForm ? 1f : 0f, bigForm ? 1f : 0f, 0.2f);

		title.AnimateTransformY(bigForm ? 283f : 173f, 0.2f);
		passphrase.AnimateTransformY(bigForm ? 101f : -11f, 0.2f);
		wordCountDropdown.AnimateTransformY(bigForm ? 308f : 198f, 0.2f);
		pastePhraseButton.AnimateTransformY(bigForm ? -236f : -126f, 0.2f);
		importButton.AnimateTransformY(bigForm ? -303f : -193f, 0.2f);

		AnimateRow(bigForm ? 12 : 20, bigForm, bigForm ? 25 : 8);
	}

	/// <summary>
	/// Animates the input fields by row
	/// </summary>
	/// <param name="row"> The number to add on by to reach the beginning of the row </param>
	/// <param name="bigForm"> Boolean that checks if it is being animated to the big version of this form </param>
	/// <param name="stoppingPoint"> The inputfield index to stop at when animating </param>
	private void AnimateRow(int row, bool addingRows, int stoppingPoint)
	{
		if (row == stoppingPoint) return;

		int newInt = addingRows ? row + 4 : row - 4;

		for (int i = 0; i < 4; i++)
		{
			if (i == 3)
				wordInputFields[i + row].AnimateScaleX(addingRows ? 1f : 0f, 0.1f, () => AnimateRow(newInt, addingRows, stoppingPoint));
			else
				wordInputFields[i + row].AnimateScaleX(addingRows ? 1f : 0f, 0.1f);
		}
	}

	/// <summary>
	/// Checks to see if all words are filled out and sets the import button to interactable if so
	/// </summary>
	private void SetButtonInteractable()
	{
		if (Animating) return;

		Button importButtonComponent = importButton.GetComponent<Button>();

		for (int i = 0; i < wordCount; i++)
		{
			if (string.IsNullOrEmpty(wordInputFields[i].GetComponent<TMP_InputField>().text))
			{
				importButtonComponent.interactable = false;
				return;
			}
		}

		importButtonComponent.interactable = true;
	}

	/// <summary>
	/// Splits the copied string on the clipboard into an array and sets them individually during the animations
	/// </summary>
	private void PastePhraseClicked()
	{
		string clipboard = ClipboardUtils.GetClipboardString();
		string[] tempArray = WalletUtils.GetMnemonicWords(clipboard);

		wordStrings = new string[24];

		for (int i = 0; i < wordStrings.Length; i++)
		{
			try { wordStrings[i] = tempArray[i]; }

			catch { wordStrings[i] = ""; }
		}

		if (tempArray.Length <= 24)
		{
			dropdownComponent.value = tempArray.Length <= 12 ? 0 : 1;
			wordCount = dropdownComponent.value == 0 ? 12 : 24;
		}

		if (clipboard != null && tempArray.Length <= wordInputFields.Length)
		{
			AnimateFormChange(wordCount != 12);
			StartWordAnimation();
		}

		else
		{
			AnimateIcon(errorIcon);
		}
	}
}
