using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddTokenPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject symbolSection;
	[SerializeField] private GameObject decimalSection;
	[SerializeField] private GameObject tokenSection;
	[SerializeField] private GameObject noTokenFound;
	[SerializeField] private GameObject addTokenButton;

	[SerializeField] private TMP_InputField addressInputField;
	[SerializeField] private GameObject loadingLine;
	
	//ADD THE ERROR AND CHECKMARK ICONS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
        GetComponent<AddTokenPopup>().OnStatusChanged += OnStatusChanged;
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(0.5f, 0.15f);
		dim.AnimateGraphic(1f, 0.15f);
		form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => title.AnimateScaleX(1f, 0.15f,
			() => addressSection.AnimateScaleX(1f, 0.15f,
			() => noTokenFound.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => addTokenButton.AnimateGraphicAndScale(1f, 1f, 0.15f, FinishedAnimating)))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		addTokenButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => addressSection.AnimateScaleX(0f, 0.15f,
			() => title.AnimateScaleX(0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => { blur.AnimateMaterialBlur(-0.5f, 0.15f); dim.AnimateGraphic(0f, 0.15f, FinishedAnimating); }))));

		noTokenFound.AnimateGraphicAndScale(0f, 0f, 0.15f);
		tokenSection.AnimateGraphicAndScale(0f, 0f, 0.15f);
		symbolSection.AnimateScaleX(0f, 0.15f);
		decimalSection.AnimateScaleX(0f, 0.15f);
	}

	public void AnimateLoadingLine(bool animateIn)
	{
		if (animateIn)
			loadingLine.SetActive(true);

		loadingLine.AnimateScaleY(animateIn ? 1f : 0f, 0.15f, () => { if (!animateIn) loadingLine.SetActive(false); });
	}

	private void AnimateNoTokenFound(bool animateIn) => noTokenFound.AnimateGraphicAndScale(animateIn ? 1f : 0f, animateIn ? 1f : 0f, 0.15f);

	private void AnimateInvalidToken(bool animateIn)
	{
		symbolSection.AnimateScaleX(animateIn ? 1f : 0f, 0.15f);
		decimalSection.AnimateScaleX(animateIn ? 1f : 0f, 0.15f);
	}

	private void AnimateValidToken(bool animateIn)
	{
		tokenSection.AnimateGraphicAndScale(animateIn ? 1f : 0f, animateIn ? 1f : 0f, 0.15f);
	}

	private void OnStatusChanged(AddTokenPopup.Status tokenPopupStatus)
    {
		AnimateLoadingLine(tokenPopupStatus == AddTokenPopup.Status.Loading);
		AnimateNoTokenFound(tokenPopupStatus == AddTokenPopup.Status.NoTokenFound);
		AnimateInvalidToken(tokenPopupStatus == AddTokenPopup.Status.InvalidToken);
		AnimateValidToken(tokenPopupStatus == AddTokenPopup.Status.ValidToken);
	}
}
