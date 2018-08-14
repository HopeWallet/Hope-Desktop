using Hope.Security.ProtectedTypes.Types;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ConfirmMnemonicMenuAnimator : UIAnimator
{
    [SerializeField] private GameObject instructions;
    [SerializeField] private GameObject wordInputField;
    [SerializeField] private GameObject nextButton;
	[SerializeField] private GameObject[] checkBoxes;
	[SerializeField] private GameObject errorIcon;

    private DynamicDataCache dynamicDataCache;
    private ConfirmMnemonicMenu confirmMnemonicMenu;

	private int wordIndex;
    private bool errorIconVisible;

    /// <summary>
    /// Adds the DynamicDataCache dependency.
    /// </summary>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    [Inject]
    public void Construct(DynamicDataCache dynamicDataCache) => this.dynamicDataCache = dynamicDataCache;

    /// <summary>
    /// Makes button interactable if the errorIcon is set to visible
    /// </summary>
    private bool ErrorIconVisible
    {
        set
        {
            errorIconVisible = value;
            if (errorIconVisible) nextButton.GetComponent<Button>().interactable = false;
        }
    }

    /// <summary>
    /// Initializes the necessary variables that haven't already been initialized in the inspector
    /// </summary>
    private void Awake()
    {
        confirmMnemonicMenu = GetComponent<ConfirmMnemonicMenu>();

        wordInputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(InputFieldChanged);
        nextButton.GetComponent<Button>().onClick.AddListener(NextButtonClicked);
    }

	/// <summary>
	/// Sets the initial word text to what the first word should be
	/// </summary>
	private void Start() => SetWordText();

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		instructions.AnimateScaleX(1f, 0.15f);
		wordInputField.AnimateScaleX(1f, 0.2f);
		nextButton.AnimateGraphicAndScale(1f, 1f, 0.25f);
		AnimateCheckboxes(0);
	}

	/// <summary>
	/// Resets the unique elements of the form back to the starting positions
	/// </summary>
	protected override void ResetElementValues()
	{
		FinishedAnimating();

		for (int i = 0; i < 4; i++)
			checkBoxes[i].SetScale(Vector2.zero);

		wordInputField.SetScale(new Vector2(0f, 1f));
		instructions.SetScale(new Vector2(0f, 1f));
		nextButton.SetGraphicAndScale(Vector2.zero);
	}

	/// <summary>
	/// Animates the checkboxes one by one
	/// </summary>
	/// <param name="index"> The index of the checkboxes array being animated </param>
	private void AnimateCheckboxes(int index)
    {
        if (index != 3)
            checkBoxes[index].AnimateScale(1f, 0.075f, () => AnimateCheckboxes(++index));
        else
            checkBoxes[index].AnimateScale(1f, 0.075f, FinishedAnimating);
    }

    /// <summary>
    /// Sets the various text to ask for the next number
    /// </summary>
    private void SetWordText()
    {
        int[] randomNums = dynamicDataCache.GetData("confirmation numbers");

        instructions.GetComponent<TextMeshProUGUI>().text = "Please enter word #" + randomNums[wordIndex] + ":";

        TMP_InputField inputField = wordInputField.GetComponent<TMP_InputField>();
        inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Word " + randomNums[wordIndex] + "...";
        inputField.text = "";
    }

    /// <summary>
    /// Animates the error icon in or out of view
    /// </summary>
    /// <param name="animatingIn"> Checks if animating the icon in or out </param>
    private void AnimateErrorIcon(bool animatingIn)
    {
        Animating = true;

        errorIcon.AnimateGraphicAndScale(animatingIn ? 1f : 0f, animatingIn ? 1f : 0f, 0.2f, () => Animating = false);

        ErrorIconVisible = animatingIn;
    }

    /// <summary>
    /// Gets the next word number of the passphrase, and animates the elements back into sight
    /// </summary>
    private void SetUpNextWord()
    {
        SetWordText();
        SetObjectPlacement(instructions);
        SetObjectPlacement(wordInputField);
        AnimateNextWord(true);
    }

    /// <summary>
    /// Animates the instructions and input field in or out depending on the boolean
    /// </summary>
    /// <param name="animatingIn"> The boolean determines if the elements are animating in or out </param>
    private void AnimateNextWord(bool animatingIn)
    {
        instructions.AnimateTransformX(animatingIn ? 0f : -350f, 0.2f);
        instructions.AnimateGraphic(animatingIn ? 1f : 0f, 0.2f);
        wordInputField.AnimateTransformX(animatingIn ? 0f : -350f, 0.2f);
        wordInputField.GetComponent<TMP_InputField>().textComponent.gameObject.AnimateGraphic(animatingIn ? 1f : 0f, 0.2f);

        if (animatingIn)
            wordInputField.AnimateGraphic(animatingIn ? 1f : 0f, 0.2f, () => Animating = false);
        else
            wordInputField.AnimateGraphic(animatingIn ? 1f : 0f, 0.2f, SetUpNextWord);
    }

    /// <summary>
    /// Sets the GameObject placement out to the right so the elements can slide back in
    /// </summary>
    /// <param name="gameObject"> The GameObject being moved </param>
    private void SetObjectPlacement(GameObject gameObject) => gameObject.transform.localPosition = new Vector2(350, gameObject.transform.localPosition.y);

    /// <summary>
    /// Sets the next button to interactable if the word input field has at least something in the input
    /// </summary>
    /// <param name="str"> The current string that is in the word input field </param>
    private void InputFieldChanged(string str)
    {
        nextButton.GetComponent<Button>().interactable = !string.IsNullOrEmpty(str);

        if (errorIconVisible) AnimateErrorIcon(false);
    }

    /// <summary>
    /// The nextButton has been clicked
    /// </summary>
    [SecureCallEnd]
    [ReflectionProtect]
    private void NextButtonClicked()
    {
        using (var word = ((ProtectedString[])dynamicDataCache.GetData("confirmation words"))[wordIndex].CreateDisposableData())
        {
            if (wordInputField.GetComponent<TMP_InputField>().text.EqualsIgnoreCase(word.Value))
            {
                if (wordIndex != checkBoxes.Length - 1)
                {
                    checkBoxes[wordIndex].transform.GetChild(1).gameObject.AnimateGraphicAndScale(1f, 1f, 0.15f);
                    Animating = true;
                    AnimateNextWord(false);
                    wordIndex++;
                }
                else
                {
                    checkBoxes[wordIndex].transform.GetChild(1).gameObject.AnimateGraphicAndScale(1f, 1f, 0.15f, confirmMnemonicMenu.LoadWallet);
                }
            }
            else
            {
                AnimateErrorIcon(true);
            }
        }
    }
}
