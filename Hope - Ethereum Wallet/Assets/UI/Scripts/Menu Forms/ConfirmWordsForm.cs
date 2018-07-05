using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmWordsForm : FormAnimation
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject instructions;
	[SerializeField] private GameObject wordInputField;
	[SerializeField] private GameObject nextButton;
	[SerializeField] private GameObject checkBoxParent;

	private GameObject[] checkBoxes;
	private int wordIndex = 0;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	protected override void InitializeElements()
	{
		checkBoxes = new GameObject[4];

		for (int i = 0; i < checkBoxes.Length; i++)
			checkBoxes[i] = checkBoxParent.transform.GetChild(i).gameObject;

		wordInputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(SetButtonInteractable);
		nextButton.GetComponent<Button>().onClick.AddListener(NextButtonClicked);

		GetRandomWord();
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(1f, 1f, 0.2f));

		instructions.AnimateScaleX(1f, 0.2f,
			() => wordInputField.AnimateScaleX(1f, 0.2f,
			() => nextButton.AnimateScaleX(1f, 0.2f, FinishedAnimatingIn)));

		AnimateCheckboxes(0, true);
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		//STILL NEED TO CODE YA DICKHEAD!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	}

	/// <summary>
	/// Animates the checkboxes one by one
	/// </summary>
	/// <param name="index"> The index of the checkboxes array being animated </param>
	/// <param name="animatingIn"> Checks if animating the boxes on or off screen </param>
	private void AnimateCheckboxes(int index, bool animatingIn)
	{
		if (index != 3)
			checkBoxes[index].AnimateScaleY(animatingIn ? 1f : 0f, 0.15f, () => AnimateCheckboxes(++index, animatingIn));
		else
			checkBoxes[index].AnimateScaleY(animatingIn ? 1f : 0f, 0.15f);
	}

	private void GetRandomWord()
	{
		int num = Random.Range(1, 12);

		instructions.GetComponent<TextMeshProUGUI>().text = "Please enter word #" + num + ":";

		TMP_InputField inputField = wordInputField.GetComponent<TMP_InputField>();
		inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Word " + num + "...";
		inputField.text = "";
	}

	/// <summary>
	/// Sets the next button to interactable if the word input field has at least something in the input
	/// </summary>
	/// <param name="str"> The current string that is in the word input field </param>
	private void SetButtonInteractable(string str) => nextButton.GetComponent<Button>().interactable = str != "" ? true : false;

	/// <summary>
	/// The nextButton has been clicked
	/// </summary>
	private void NextButtonClicked()
	{
		//Check if word is correct

		//if (wordIsCorrect)
		//{
		if (wordIndex != 3)
		{
			checkBoxes[wordIndex].transform.GetChild(1).gameObject.AnimateGraphicAndScale(1f, 1f, 0.15f);
			Animating = true;
			AnimateNextWord(false);
			wordIndex++;
		}

		else
			checkBoxes[wordIndex].transform.GetChild(1).gameObject.AnimateGraphicAndScale(1f, 1f, 0.15f, DisableMenu);
		//}
	}

	/// <summary>
	/// Gets the next word number of the passphrase, and animates the elements back into sight
	/// </summary>
	private void SetUpNextWord()
	{
		GetRandomWord();
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
}
