using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Random = System.Random;
using Nethereum.HdWallet;
using NBitcoin;

public class PassphraseForm : FormAnimation
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject passphrase;
	[SerializeField] private GameObject[] wordObjects;
	[SerializeField] private GameObject[] words;
	[SerializeField] private GameObject generateNewButton;
	[SerializeField] private GameObject copyAllButton;
	[SerializeField] private GameObject confirmButton;

	private string[] mnemonicWords;

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

	#region Animating

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

	protected override void AnimateOut()
	{
	}

	/// <summary>
	/// Animates the word objects scaleX by row
	/// </summary>
	/// <param name="row">The int to be added by for each row</param>
	private void AnimatePassphrase(int row)
	{
		for (int x = 0; x < 3; x++)
		{
			if (x == 2 && row == 9) wordObjects[x + row].AnimateScaleX(1f, 0.2f, () => FinishedAnimatingIn());

			else if (x == 2) wordObjects[x + row].AnimateScaleX(1f, 0.2f, () => AnimatePassphrase(row += 3));

			else wordObjects[x + row].AnimateScaleX(1f, 0.2f);
		}
	}

	#region Word Generation Animation

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
	/// If there are still words left to animate, it calls the CrunchWord animation
	/// </summary>
	/// <param name="wordList">The randomized list of words</param>
	/// <param name="index">The index that is being animated</param>
	private void ProcessWordAnimation(List<GameObject> wordList, int index)
	{
		if (index < wordList.Count)
			CrunchWord(wordList, index);
		else
			Animating = false;
	}

	/// <summary>
	/// Scales the word's X value to zero
	/// </summary>
	/// <param name="wordList">The randomized list of words</param>
	/// <param name="index">The index that is being animated</param>
	private void CrunchWord(List<GameObject> wordList, int index)
	{
		wordList[index].AnimateScaleX(0f, 0.05f, () => ExpandWord(wordList, index));
	}

	/// <summary>
	/// Scales the word's X value back to 1
	/// </summary>
	/// <param name="wordList">The randomized list of words</param>
	/// <param name="index">The index that is being animated</param>
	private void ExpandWord(List<GameObject> wordList, int index)
	{
		wordList[index].GetComponent<TextMeshProUGUI>().text = mnemonicWords[index];
		wordList[index].AnimateScaleX(1f, 0.05f, () => ProcessWordAnimation(wordList, ++index));
	}

	#endregion

	#endregion

	#region Button Clicks

	public void GenerateNewClicked()
	{
		GenerateMnemonicPhrase();
		StartWordAnimation();
	}

	public void CopyAllClicked()
	{
		string entirePassphrase = "";

		for (int i = 0; i < 12; i++)
			entirePassphrase += words[i].GetComponent<TextMeshProUGUI>().text + " ";

		ClipboardUtils.CopyToClipboard(entirePassphrase);
	}

	#endregion

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
}
