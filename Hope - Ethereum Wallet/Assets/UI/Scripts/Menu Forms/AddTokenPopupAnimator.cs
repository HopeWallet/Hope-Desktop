using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddTokenPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject addressField;
	[SerializeField] private GameObject addTokenButton;
	[SerializeField] private GameObject errorIcon;
	[SerializeField] private GameObject checkMarkIcon;

	private bool errorIconVisible;

	/// <summary>
	/// Makes button interactable if the errorIcon is set to visible
	/// </summary>
	private bool ErrorIconVisible
	{
		set
		{
			errorIconVisible = value;
			if (errorIconVisible) addTokenButton.GetComponent<Button>().interactable = false;
		}
	}

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
		addTokenButton.GetComponent<Button>().onClick.AddListener(AddTokenClicked);
		addressField.GetComponent<TMP_InputField>().onValueChanged.AddListener(AddressFieldChanged);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(1.25f, 0.2f);
		dim.AnimateGraphic(1f, 0.2f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateScaleX(1f, 0.1f,
			() => addressField.AnimateScaleX(1f, 0.1f,
			() => addTokenButton.AnimateScaleX(1f, 0.1f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		title.AnimateScaleX(0f, 0.1f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => { blur.AnimateMaterialBlur(0f, 0.15f); dim.AnimateGraphic(0f, 0.15f, FinishedAnimating); }));

		addTokenButton.AnimateScaleX(0f, 0.1f,
			() => addressField.AnimateScaleX(0f, 0.1f));
	}

	/// <summary>
	/// Sets the addTokenButton interactable to true or false depending on if the text is empty in the addressField
	/// </summary>
	/// <param name="str"> The current string in the addressField </param>
	private void AddressFieldChanged(string str)
	{
		addTokenButton.GetComponent<Button>().interactable = !string.IsNullOrEmpty(str);

		if (errorIconVisible) AnimateErrorIcon(false);
	}

	/// <summary>
	/// addTokenButton is clicked and checks if the inputted text is a valid token address
	/// </summary>
	private void AddTokenClicked()
	{
		//if (not a valid token address)
			AnimateErrorIcon(true);
	}

	/// <summary>
	/// Animates the errorIcon in or out of view
	/// </summary>
	/// <param name="animatingIn"> Checks if animating the errorIcon in or out </param>
	private void AnimateErrorIcon(bool animatingIn)
	{
		Animating = true;

		errorIcon.AnimateGraphicAndScale(animatingIn ? 1f : 0f, animatingIn ? 1f : 0f, 0.2f, () => Animating = false);

		ErrorIconVisible = animatingIn;
	}

	/// <summary>
	/// Animates the checkMarkIcon in
	/// </summary>
	private void AnimateCheckMarkIcon()
	{
		Animating = true;
		addTokenButton.GetComponent<Button>().interactable = false;

		checkMarkIcon.transform.localScale = new Vector3(0, 0, 1);

		checkMarkIcon.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => checkMarkIcon.AnimateScaleX(1.01f, 0.5f,
			() => checkMarkIcon.AnimateGraphic(0f, 0.2f, FinishedCheckMarkAnimation)));
	}

	/// <summary>
	/// Finishes the checkMarkIcon animation and sets the text back to empty
	/// </summary>
	private void FinishedCheckMarkAnimation()
	{
		addressField.GetComponent<TMP_InputField>().text = "";
		Animating = false;
	}
}
