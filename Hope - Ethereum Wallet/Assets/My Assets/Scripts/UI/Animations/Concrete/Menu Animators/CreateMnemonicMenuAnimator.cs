using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Random = System.Random;
using Zenject;
using Hope.Utils.Ethereum;

/// <summary>
/// The animator class of the CreateMnemonicMenu
/// </summary>
public sealed class CreateMnemonicMenuAnimator : MenuAnimator
{
	[SerializeField] private Transform passphrase;
	[SerializeField] private GameObject generateNewButton;
	[SerializeField] private GameObject copyPhraseButton;
	[SerializeField] private GameObject nextButton;
	[SerializeField] private GameObject checkMarkIcon;

	private readonly List<GameObject> words = new List<GameObject>();
	private readonly List<GameObject> wordFields = new List<GameObject>();

	private string[] mnemonicWords;
	private bool animatingIcon;

	private DynamicDataCache dynamicDataCache;

	[Inject]
	public void Construct(DynamicDataCache dynamicDataCache) => this.dynamicDataCache = dynamicDataCache;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	private void Awake()
	{
		for (int i = 0; i < 12; i++)
		{
			wordFields.Add(passphrase.transform.GetChild(i).gameObject);
			words.Add(wordFields[i].transform.GetChild(1).GetChild(0).gameObject);
			wordFields[i].GetComponent<Image>().color = UIColors.Green;
		}
	}

	/// <summary>
	/// Adds the animation methods to the button listeners of the CreateMnemonicMenu.
	/// </summary>
	private void Start()
	{
		generateNewButton.transform.GetComponent<Button>().onClick.AddListener(StartWordAnimation);
		copyPhraseButton.transform.GetComponent<Button>().onClick.AddListener(() => { if (!animatingIcon) AnimateCheckMarkIcon(); });
		nextButton.GetComponent<Button>().onClick.AddListener(() => Animating = true);
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		float duration = 0.24f;
		for (int i = 0; i < 12; i++)
		{
			passphrase.GetChild(i).gameObject.AnimateScaleX(1f, duration);

			if (i == 2 || i == 5 || i == 8)
				duration += 0.03f;
		}

		generateNewButton.AnimateGraphicAndScale(1f, 1f, 0.35f);
		copyPhraseButton.AnimateGraphicAndScale(1f, 1f, 0.35f);
		nextButton.AnimateGraphicAndScale(1f, 1f, 0.4f, FinishedAnimating);
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		for (int i = 0; i < 12; i++)
			passphrase.GetChild(i).gameObject.AnimateScaleX(0f, 0.3f);

		generateNewButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		copyPhraseButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		nextButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the word objects scaleX by row
	/// </summary>
	/// <param name="row"> The int to be added by for each row </param>
	private void AnimatePassphrase(int row)
	{
		for (int x = 0; x < 3; x++)
		{
			if (x == 2 && row == 9) wordFields[x + row].AnimateScaleX(1f, 0.08f, FinishedAnimating);

			else if (x == 2) wordFields[x + row].AnimateScaleX(1f, 0.08f, () => AnimatePassphrase(row += 3));

			else wordFields[x + row].AnimateScaleX(1f, 0.08f);
		}
	}

	/// <summary>
	/// Animates the check mark icon on and off screen
	/// </summary>
	private void AnimateCheckMarkIcon()
	{
		animatingIcon = true;
		checkMarkIcon.transform.localScale = new Vector3(0, 0, 1);

		checkMarkIcon.AnimateGraphicAndScale(1f, 1f, 0.15f);
		CoroutineUtils.ExecuteAfterWait(0.6f, () => { if (checkMarkIcon != null) checkMarkIcon.AnimateGraphic(0f, 0.25f, () => animatingIcon = false); });
	}

	/// <summary>
	/// Initializes the randomized list of words and starts the series of random word animations
	/// </summary>
	[SecureCallEnd]
	[ReflectionProtect]
	private void StartWordAnimation()
	{
        mnemonicWords = WalletUtils.GetMnemonicWords(dynamicDataCache.GetData("mnemonic"));

		wordFields.ForEach(f => f.AnimateColor(UIColors.White, 0.1f));

		Animating = true;
		Random rand = new Random();

		List<GameObject> randomizedWordList = new List<GameObject>(words);
		randomizedWordList.Sort((_, __) => rand.Next(-1, 1));

		ProcessWordAnimation(randomizedWordList, 0);
	}

	/// <summary>
	/// Scales the word's X value back to 1
	/// </summary>
	/// <param name="randomizedWordList"> The randomized list of words </param>
	/// <param name="index"> The index that is being animated </param>
	private void AnimateWord(List<GameObject> randomizedWordList, int index)
	{
		var InputField = randomizedWordList[index].transform.parent.parent.GetComponent<TMP_InputField>();

		InputField.gameObject.AnimateColor(UIColors.Green, 0.1f);
		randomizedWordList[index].AnimateGraphicAndScale(0f, 0f, 0.05f, () =>
		{
			InputField.text = mnemonicWords[words.IndexOf(randomizedWordList[index])];
			randomizedWordList[index].AnimateGraphicAndScale(1f, 1f, 0.05f, () => ProcessWordAnimation(randomizedWordList, ++index));
		});
	}

	/// <summary>
	/// If there are still words left to animate, it calls the CrunchWord animation again
	/// </summary>
	/// <param name="randomizedWordList"> The randomized list of words </param>
	/// <param name="index"> The index that is being animated </param>
	private void ProcessWordAnimation(List<GameObject> randomizedWordList, int index)
	{
		if (index < randomizedWordList.Count)
		{
			AnimateWord(randomizedWordList, index);
		}
		else
		{
			Animating = false;

			foreach (GameObject wordField in wordFields)
				wordField.transform.GetChild(1).GetChild(0).transform.localScale = Vector2.one;
		}
	}
}
