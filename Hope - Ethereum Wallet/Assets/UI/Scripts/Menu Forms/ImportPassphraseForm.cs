using UnityEngine;
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
	private int wordCount;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	protected override void InitializeElements()
	{
		backButton1 = form1.transform.GetChild(0).gameObject;
		backButton2 = form2.transform.GetChild(0).gameObject;

		wordInputField = new GameObject[24];
		wordTextObjects = new GameObject[24];

		for (int i = 0; i < wordInputField.Length; i++)
		{
			wordInputField[i] = passphrase.transform.GetChild(i).gameObject;
			wordTextObjects[i] = wordInputField[i].transform.GetChild(0).GetChild(2).gameObject;

			wordInputField[i].GetComponent<TMP_InputField>().onValueChanged.AddListener((str) => SetButtonInteractable());
		}

		importButton.GetComponent<Button>().interactable = false;

		dropdownComponent = wordCountDropdown.GetComponent<TMP_Dropdown>();
		dropdownComponent.onValueChanged.AddListener(PassphraseWordCountChanged);
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

		string[] tempArray = clipboard.GetMnemonicWords();

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

		if (clipboard != null && tempArray.Length <= wordInputField.Length)
		{
			AnimateFormChange(wordCount == 12 ? false : true);
			StartWordAnimation();
		}

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

		for (int i = 0; i < wordCount; i++)
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
	private void PassphraseWordCountChanged(int value)
	{
		if (value == 0)
		{
			wordCount = 12;
			AnimateFormChange(false);
		}

		else
		{
			wordCount = 24;
			AnimateFormChange(true);
		}
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
		backButton2.AnimateGraphic(bigForm ? 1f : 0f, 0.2f);

		title.AnimateTransformY(bigForm ? 283f : 173f, 0.2f);
		passphrase.AnimateTransformY(bigForm ? 101f : -11f, 0.2f);
		wordCountDropdown.AnimateTransformY(bigForm ? 308f : 198f, 0.2f);
		pastePhraseButton.AnimateTransformY(bigForm ? -236f : -126f, 0.2f);
		importButton.AnimateTransformY(bigForm ? -303f : -193f, 0.2f);

		AnimateRow(bigForm ? 12 : 20, bigForm);
	}

	/// <summary>
	/// Animates the input fields by row
	/// </summary>
	/// <param name="row"> The number to add on by to reach the beginning of the row </param>
	/// <param name="bigForm"> Boolean that checks if it is being animated to the big version of this form </param>
	private void AnimateRow(int row, bool bigForm)
	{
		if (row == 25 || row == 8) return;

		int newInt = bigForm ? row + 4 : row - 4;

		for (int i = 0; i < 4; i++)
		{
			if (i == 3)
				wordInputField[i + row].AnimateGraphicAndScale(bigForm ? 1f : 0f, bigForm ? 1f : 0f, 0.1f, () => AnimateRow(newInt, bigForm));

			else
				wordInputField[i + row].AnimateGraphicAndScale(bigForm ? 1f : 0f, bigForm ? 1f : 0f, 0.1f);
		}
	}
}
