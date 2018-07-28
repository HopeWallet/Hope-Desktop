using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Random = System.Random;
using Zenject;
using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.EthereumUtils;

/// <summary>
/// Class which animates the CreateMnemonicMenu.
/// </summary>
public class CreateMnemonicMenuAnimator : UIAnimator
{

    [SerializeField] private GameObject form;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject passphrase;
    [SerializeField] private GameObject generateNewButton;
    [SerializeField] private GameObject copyAllButton;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject checkMarkIcon;

    private readonly List<GameObject> words = new List<GameObject>();
    private readonly List<GameObject> wordObjects = new List<GameObject>();

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
            wordObjects.Add(passphrase.transform.GetChild(i).gameObject);
            words.Add(wordObjects[i].transform.GetChild(2).gameObject);
        }
    }

    /// <summary>
    /// Adds the animation methods to the button listeners of the CreateMnemonicMenu.
    /// </summary>
    private void Start()
    {
        var createMnemonicMenu = GetComponent<CreateMnemonicMenu>();
        createMnemonicMenu.generateNewWords.onClick.AddListener(StartWordAnimation);
        createMnemonicMenu.copyMnemonic.onClick.AddListener(AnimateCheckMarkIcon);
    }

    /// <summary>
    /// Animates the UI elements of the form into view
    /// </summary>
    protected override void AnimateIn()
    {
        form.AnimateGraphicAndScale(1f, 1f, 0.2f,
            () => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f, 
			() => AnimatePassphrase(0)));

        generateNewButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
            () => copyAllButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
            () => confirmButton.AnimateGraphicAndScale(1f, 1f, 0.2f)));       
    }

    /// <summary>
    /// Animates the UI elements of the form out of view
    /// </summary>
    protected override void AnimateOut()
    {
        title.AnimateGraphicAndScale(0f, 0f, 0.2f,
            () => form.AnimateGraphicAndScale(0f, 0f, 0.2f));

        generateNewButton.AnimateGraphicAndScale(0f, 0f, 0.2f);
        copyAllButton.AnimateGraphicAndScale(0f, 0f, 0.2f,
            () => confirmButton.AnimateGraphicAndScale(0f, 0f, 0.2f, FinishedAnimating));

        for (int i = 0; i < wordObjects.Count; i++)
            wordObjects[i].AnimateScaleX(0f, 0.2f);
    }

    /// <summary>
    /// Animates the word objects scaleX by row
    /// </summary>
    /// <param name="row"> The int to be added by for each row </param>
    private void AnimatePassphrase(int row)
    {
        for (int x = 0; x < 3; x++)
        {
            if (x == 2 && row == 9) wordObjects[x + row].AnimateScaleX(1f, 0.2f, FinishedAnimating);

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
            () => checkMarkIcon.AnimateScaleX(1.01f, 0.5f,
            () => checkMarkIcon.AnimateGraphic(0f, 0.5f,
            () => copyButtonComponent.interactable = true)));
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
    private void CrunchWord(List<GameObject> randomizedList, int index) => randomizedList[index].AnimateScaleX(0f, 0.05f, () => ExpandWord(randomizedList, index));

    /// <summary>
    /// Scales the word's X value back to 1
    /// </summary>
    /// <param name="randomizedList"> The randomized list of words </param>
    /// <param name="index"> The index that is being animated </param>
    private void ExpandWord(List<GameObject> randomizedList, int index)
    {
        randomizedList[index].GetComponent<TextMeshProUGUI>().text = mnemonicWords[words.IndexOf(randomizedList[index])];
        randomizedList[index].AnimateScaleX(1f, 0.05f, () => ProcessWordAnimation(randomizedList, ++index));
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
            Animating = false;
    }
}
