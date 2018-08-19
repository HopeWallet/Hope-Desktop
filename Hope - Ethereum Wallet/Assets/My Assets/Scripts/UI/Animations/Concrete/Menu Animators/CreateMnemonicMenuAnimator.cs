using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Random = System.Random;
using Zenject;
using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.Ethereum;

/// <summary>
/// Class which animates the CreateMnemonicMenu.
/// </summary>
public class CreateMnemonicMenuAnimator : UIAnimator
{
	[SerializeField] private GameObject passphrase;
	[SerializeField] private GameObject generateNewButton;
	[SerializeField] private GameObject copyAllButton;
	[SerializeField] private GameObject nextButton;
	[SerializeField] private GameObject checkMarkIcon;

	private readonly List<GameObject> words = new List<GameObject>();
	private readonly List<GameObject> wordFields = new List<GameObject>();

	private string[] mnemonicWords;

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
		}
	}

	/// <summary>
	/// Adds the animation methods to the button listeners of the CreateMnemonicMenu.
	/// </summary>
	private void Start()
	{
		generateNewButton.transform.GetComponent<Button>().onClick.AddListener(StartWordAnimation);
		copyAllButton.transform.GetComponent<Button>().onClick.AddListener(AnimateCheckMarkIcon);
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		FinishedAnimating();
		//AnimatePassphrase(0);
		//generateNewButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
		//copyAllButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
		//nextButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}


	/// <summary>
	/// Resets the unique elements of the form back to the starting positions
	/// </summary>
	protected override void ResetElementValues()
	{
		FinishedAnimating();

		//for (int i = 0; i < 12; i++)
		//	wordObjects[i].SetScale(new Vector2(0f, 1f));

		//generateNewButton.SetGraphicAndScale(Vector2.zero);
		//copyAllButton.SetGraphicAndScale(Vector2.zero);
		//nextButton.SetGraphicAndScale(Vector2.zero);
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
		Animating = true;

		checkMarkIcon.transform.localScale = new Vector3(0, 0, 1);

		checkMarkIcon.AnimateGraphicAndScale(1f, 1f, 0.15f);
		CoroutineUtils.ExecuteAfterWait(0.6f, () => checkMarkIcon.AnimateGraphic(0f, 0.25f,
			() => Animating = false));
	}

	/// <summary>
	/// Initializes the randomized list of words and starts the series of random word animations
	/// </summary>
	[SecureCallEnd]
	[ReflectionProtect]
	private void StartWordAnimation()
	{
		using (var mnemonic = (dynamicDataCache.GetData("mnemonic") as ProtectedString)?.CreateDisposableData())
			mnemonicWords = WalletUtils.GetMnemonicWords(mnemonic.Value);

		Animating = true;
		Random rand = new Random();

		List<GameObject> randomizedList = new List<GameObject>(words);
		randomizedList.Sort((_, __) => rand.Next(-1, 1));

		ProcessWordAnimation(randomizedList, 0);
	}

	/// <summary>
	/// Scales the word's X value to zero
	/// </summary>
	/// <param name="randomizedList"> The randomized list of words </param>
	/// <param name="index"> The index that is being animated </param>
	private void CrunchWord(List<GameObject> randomizedList, int index) => randomizedList[index].AnimateGraphicAndScale(0f, 0f, 0.05f, () => ExpandWord(randomizedList, index));

	/// <summary>
	/// Scales the word's X value back to 1
	/// </summary>
	/// <param name="randomizedList"> The randomized list of words </param>
	/// <param name="index"> The index that is being animated </param>
	private void ExpandWord(List<GameObject> randomizedList, int index)
	{
		randomizedList[index].transform.parent.parent.GetComponent<TMP_InputField>().text = mnemonicWords[words.IndexOf(randomizedList[index])];
		randomizedList[index].AnimateGraphicAndScale(1f, 1f, 0.05f, () => ProcessWordAnimation(randomizedList, ++index));
	}

	/// <summary>
	/// If there are still words left to animate, it calls the CrunchWord animation again
	/// </summary>
	/// <param name="randomizedList"> The randomized list of words </param>
	/// <param name="index"> The index that is being animated </param>
	private void ProcessWordAnimation(List<GameObject> randomizedList, int index)
	{
		if (index < randomizedList.Count)
			CrunchWord(randomizedList, index);
		else
		{
			Animating = false;

			for (int i = 0; i < 12; i++)
				wordFields[i].transform.GetChild(1).GetChild(0).transform.localScale = Vector2.one;
		}
	}
}
