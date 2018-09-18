using UnityEngine;
using Hope.Utils.Ethereum;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// The animator class of the ImportMnemonicMenu
/// </summary>
public sealed class ImportMnemonicMenuAnimator : MenuAnimator
{
	[SerializeField] private Transform passphrase;
	[SerializeField] private GameObject wordCountSection;
	[SerializeField] private GameObject pastePhraseButton;
	[SerializeField] private GameObject nextButton;
	[SerializeField] private GameObject pasteButtonCheckMarkIcon;
	[SerializeField] private GameObject pasteButtonErrorIcon;
	[SerializeField] private GameObject loadingIcon;

	public GameObject nextButtonErrorIcon;
	public GameObject nextButtonErrorMessage;

	private HopeInputField[] wordInputFields;
	private GameObject[] wordTextObjects;

	private ImportMnemonicMenu importMnemonicMenu;

	private string[] wordStrings;
	private int wordCount = 12;
	private bool animatingIcon;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		importMnemonicMenu = transform.GetComponent<ImportMnemonicMenu>();
		importMnemonicMenu.OnWalletLoading += CreateWallet;

		wordInputFields = new HopeInputField[24];
		wordTextObjects = new GameObject[24];

		for (int i = 0; i < wordInputFields.Length; i++)
			SetInputFieldVariables(i);

		wordCountSection.GetComponent<SingleChoiceButtonsBase>().OnButtonChanged += PassphraseWordCountChanged;
		pastePhraseButton.GetComponent<Button>().onClick.AddListener(PastePhraseClicked);
		importMnemonicMenu.LastSelectableField = wordInputFields[11].InputFieldBase;
		nextButton.GetComponent<Button>().onClick.AddListener(() => Animating = true);
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateIn()
	{
		base.AnimateIn();

		float duration = 0.25f;
		for (int i = 0; i < 5; i++)
		{
			wordCountSection.transform.GetChild(i).gameObject.AnimateScaleX(1f, duration);
			duration += 0.01f;
		}

		pastePhraseButton.AnimateGraphicAndScale(1f, 1f, 0.3f);

		for (int i = 0; i < 24; i++)
		{
			wordInputFields[i].gameObject.AnimateScaleX(1f, duration);

			if (i == 5 || i == 11 || i == 17)
				duration += 0.03f;
		}

		nextButton.AnimateGraphicAndScale(1f, 1f, 0.4f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		base.AnimateOut();

		for (int i = 0; i < 5; i++)
			wordCountSection.transform.GetChild(i).gameObject.AnimateScaleX(0f, 0.3f);

		pastePhraseButton.AnimateGraphicAndScale(0f, 0f, 0.3f);

		for (int i = 0; i < 24; i++)
			wordInputFields[i].gameObject.AnimateScaleX(0f, 0.3f);

		nextButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Sets the input field variables at a given index
	/// </summary>
	/// <param name="i"> The index of the input field in the hiearchy </param>
	private void SetInputFieldVariables(int i)
	{
		wordInputFields[i] = passphrase.GetChild(i).GetComponent<HopeInputField>();
		wordTextObjects[i] = wordInputFields[i].transform.GetChild(1).GetChild(0).gameObject;

		wordInputFields[i].OnInputUpdated += _ => CheckIfValidInput(i);

		if (wordInputFields[i].InputFieldBase.interactable)
			importMnemonicMenu.SelectableFields.Add(wordInputFields[i].InputFieldBase);
	}

	/// <summary>
	/// Starts the series of word animations
	/// </summary>
	private void StartWordAnimation()
	{
		wordInputFields.ForEach(i => i.Text = string.Empty);

		foreach (GameObject textObject in wordTextObjects)
		{
			textObject.transform.localScale = new Vector2(0f, 0f);
			textObject.AnimateColor(new Color(0.85f, 0.85f, 0.85f, 0f), 0.1f);
		}

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
		wordTextObjects[index].AnimateGraphicAndScale(1f, 1f, 0.1f, () => ProcessWordAnimation(++index));
	}

	/// <summary>
	/// If there are still words left to animate, it calls the CrunchWord animation again
	/// </summary>
	/// <param name="index"> The index of the current input field </param>
	private void ProcessWordAnimation(int index)
	{
		if (index < wordStrings.Length && index < 24)
		{
			AnimateWord(index);
		}

		else
		{
			for (int i = index; i < wordCount; i++)
			{
				wordTextObjects[i].transform.localScale = Vector2.one;
				wordTextObjects[i].GetComponent<Text>().color = UIColors.White;
			}

			Animating = false;
			SetButtonInteractable();
		}
	}

	/// <summary>
	/// Animates an icon in and out of view
	/// </summary>
	/// <param name="icon"> The GameObject that is being animated </param>
	public void AnimateIcon(GameObject icon)
	{
		animatingIcon = true;
		icon.transform.localScale = new Vector3(0, 0, 1);

		icon.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => CoroutineUtils.ExecuteAfterWait(0.6f, () => { if (icon != null) icon.AnimateGraphic(0f, 0.25f, () => animatingIcon = false); }));
	}

	/// <summary>
	/// Checks the value of currently selected word count
	/// </summary>
	/// <param name="value"> The value of the dropdown </param>
	private void PassphraseWordCountChanged(int value)
	{
		value = 12 + (value * 3);

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
			InputField baseInputField = wordInputFields[i].InputFieldBase;

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

		var compatibleWords = wordInputFields.Where(b => b.InputFieldBase.interactable && !b.Error).ToList();

		nextButton.GetComponent<Button>().interactable = compatibleWords.Count == wordCount;
	}

	/// <summary>
	/// Splits the copied string on the clipboard into an array and sets them individually during the animations
	/// </summary>
	private void PastePhraseClicked()
	{
		string clipboard = ClipboardUtils.GetClipboardString();
		wordStrings = WalletUtils.GetMnemonicWords(clipboard);

		if (!string.IsNullOrEmpty(clipboard.Trim()))
		{
			int numOfWords = wordStrings.Length;
			wordCountSection.GetComponent<SingleChoiceButtonsBase>().ButtonClicked(numOfWords <= 12 ? 0 : numOfWords <= 15 ? 1 : numOfWords <= 18 ? 2 : numOfWords <= 21 ? 3 : 4);

			AnimateIcon(pasteButtonCheckMarkIcon);
			StartWordAnimation();
		}

		else
		{
			if (!animatingIcon)
				AnimateIcon(pasteButtonErrorIcon);
		}
	}

	private void CreateWallet()
	{
		nextButton.AnimateGraphicAndScale(0f, 0f, 0.15f);
		loadingIcon.SetActive(true);
		loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.15f);
	}
}
