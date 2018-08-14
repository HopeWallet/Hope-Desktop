using UnityEngine;
using TMPro;
using Hope.Utils.Ethereum;
using UnityEngine.UI;

/// <summary>
/// Class used for animating the ImportMnemonicMenu.
/// </summary>
public class ImportMnemonicMenuAnimator : UIAnimator
{
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
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		throw new System.NotImplementedException();
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
			wordInputFields[index].transform.GetChild(0).GetChild(0).transform.localScale = Vector3.one;
		});
	}

	/// <summary>
	/// If there are still words left to animate, it calls the CrunchWord animation again
	/// </summary>
	/// <param name="index"> The index that is being animated </param>
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

		SetButtonInteractable();
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
		wordStrings = WalletUtils.GetMnemonicWords(clipboard);

		int words = wordStrings.Length;

		if (clipboard != null && words <= 24)
		{
			dropdownComponent.value = words <= 12 ? 0 : 1;

			if (words != 12 || words != 24)
				importButton.GetComponent<Button>().interactable = false;

			for (int i = 0; i < 24; i++)
				wordInputFields[i].GetComponent<TMP_InputField>().text = "";

			StartWordAnimation();
		}

		else
		{
			AnimateIcon(errorIcon);
		}
	}
}
