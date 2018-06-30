using UnityEngine;
using TMPro;
using Hope.Utils.EthereumUtils;

public class ImportPassphraseForm : FormAnimation
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject passphrase;
	[SerializeField] private GameObject[] wordInputField;
	[SerializeField] private GameObject[] wordTextObjects;
	[SerializeField] private GameObject pastePhraseButton;
	[SerializeField] private GameObject importButton;
	[SerializeField] private GameObject checkMarkIcon;
	[SerializeField] private GameObject errorIcon;

	private string[] wordStrings;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	protected override void InitializeElements()
	{
		wordInputField = new GameObject[12];
		wordTextObjects = new GameObject[12];

		for (int i = 0; i < 12; i++)
		{
			wordInputField[i] = passphrase.transform.GetChild(i).gameObject;
			wordTextObjects[i] = wordInputField[i].transform.GetChild(2).gameObject;
		}
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		//STILL NEED TO CODE YA DICKHEAD!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
	private void PastePhraseClicked()
	{
		string clipboard = GUIUtility.systemCopyBuffer;

		if (clipboard != null) wordStrings = clipboard.GetMnemonicWords();

		else Debug.LogError("Nothing copied to clipboard! (add this message to a UI popup for user to see)"); //Add this message to a UI popup for the user to see if nothing is copied to the clipboard

		StartWordAnimation();	
	}

	/// <summary>
	/// Starts the series of word animations
	/// </summary>
	private void StartWordAnimation()
	{
		Animating = true;
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
		wordTextObjects[index].GetComponent<TextMeshProUGUI>().text = wordStrings[index];
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
		else
			Animating = false;
	}
}
