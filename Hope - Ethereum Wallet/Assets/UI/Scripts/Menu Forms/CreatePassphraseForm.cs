using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Nethereum.HdWallet;
using NBitcoin;
using Random = System.Random;

public class CreatePassphraseForm : FormAnimation
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject passphrase;
	[SerializeField] private GameObject[] wordObjects;
	[SerializeField] private GameObject[] words;
	[SerializeField] private GameObject generateNewButton;
	[SerializeField] private GameObject copyAllButton;
	[SerializeField] private GameObject confirmButton;
	[SerializeField] private GameObject checkMarkIcon;

	private string[] mnemonicWords;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	protected override void InitializeElements()
	{
		wordObjects = new GameObject[12];
		words = new GameObject[12];

		for (int i = 0; i < 12; i++)
		{
			wordObjects[i] = passphrase.transform.GetChild(i).gameObject;
			words[i] = wordObjects[i].transform.GetChild(2).gameObject;
		}
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f));

		generateNewButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => copyAllButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => confirmButton.AnimateGraphicAndScale(1f, 1f, 0.2f)));

		SetWords();
		AnimatePassphrase(0);
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		//STILL NEED TO CODE YA DICKHEAD!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	}

	/// <summary>
	/// Animates the word objects scaleX by row
	/// </summary>
	/// <param name="row"> The int to be added by for each row </param>
	private void AnimatePassphrase(int row)
	{
		for (int x = 0; x < 3; x++)
		{
			if (x == 2 && row == 9) wordObjects[x + row].AnimateScaleX(1f, 0.2f, () => FinishedAnimatingIn());

			else if (x == 2) wordObjects[x + row].AnimateScaleX(1f, 0.2f, () => AnimatePassphrase(row += 3));

			else wordObjects[x + row].AnimateScaleX(1f, 0.2f);
		}
	}

	/// <summary>
	/// Animates the check mark icon on and off screen
	/// </summary>
	private void AnimateCheckMarkIcon()
	{
		Button copyButtonComponent = copyAllButton.GetComponent<Button>();
		copyButtonComponent.interactable = false;

		checkMarkIcon.transform.localScale = new Vector3(0, 0, 1);

		checkMarkIcon.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => checkMarkIcon.AnimateScaleX(1.01f, 1f,
			() => checkMarkIcon.AnimateGraphic(0f, 1f,
			() => copyButtonComponent.interactable = true)));
	}

	/// <summary>
	/// Initializes the randomized list of words and starts the series of random word animations
	/// </summary>
	private void StartWordAnimation()
	{
		Animating = true;
		Random rand = new Random();

		List<GameObject> wordList = words.ToList();
		wordList.Sort((x, y) => rand.Next(-1, 1));

		ProcessWordAnimation(wordList, 0);
	}

	/// <summary>
	/// Scales the word's X value to zero
	/// </summary>
	/// <param name="wordList"> The randomized list of words </param>
	/// <param name="index"> The index that is being animated </param>
	private void CrunchWord(List<GameObject> wordList, int index)
	{
		wordList[index].AnimateScaleX(0f, 0.05f, () => ExpandWord(wordList, index));
	}

	/// <summary>
	/// Scales the word's X value back to 1
	/// </summary>
	/// <param name="wordList"> The randomized list of words </param>
	/// <param name="index"> The index that is being animated </param>
	private void ExpandWord(List<GameObject> wordList, int index)
	{
		wordList[index].GetComponent<TextMeshProUGUI>().text = mnemonicWords[index];
		wordList[index].AnimateScaleX(1f, 0.05f, () => ProcessWordAnimation(wordList, ++index));
	}

	/// <summary>
	/// If there are still words left to animate, it calls the CrunchWord animation again
	/// </summary>
	/// <param name="wordList"> The randomized list of words </param>
	/// <param name="index"> The index that is being animated </param>
	private void ProcessWordAnimation(List<GameObject> wordList, int index)
	{
		if (index < wordList.Count)
			CrunchWord(wordList, index);
		else
			Animating = false;
	}

	/// <summary>
	/// Generates a random passphrase and animates the words
	/// </summary>
	public void GenerateNewClicked()
	{
		GenerateMnemonicPhrase();
		StartWordAnimation();
	}

	/// <summary>
	/// Merges all the words to one giant string and copies it to the clipboard
	/// </summary>
	public void CopyAllClicked()
	{
		CopyPassphraseToClipboard();
		AnimateCheckMarkIcon();
	}

	/// <summary>
	/// Generates a new passphrase onto the mnemonicWords array
	/// </summary>
	private void GenerateMnemonicPhrase()
	{
		mnemonicWords = new Wallet(Wordlist.English, WordCount.Twelve).Words;
	}

	/// <summary>
	/// Sets the array of words to the newly generated passphrase
	/// </summary>
	private void SetWords()
	{
		GenerateMnemonicPhrase();

		for (int i = 0; i < 12; i++)
			words[i].GetComponent<TextMeshProUGUI>().text = mnemonicWords[i];
	}

	/// <summary>
	/// Combines all the words in the array into one long string and copies it to the clipboard
	/// </summary>
	private void CopyPassphraseToClipboard()
	{
		string entirePassphrase = "";

		for (int i = 0; i < 12; i++)
			entirePassphrase += words[i].GetComponent<TextMeshProUGUI>().text + " ";

		ClipboardUtils.CopyToClipboard(entirePassphrase);
	}

}
