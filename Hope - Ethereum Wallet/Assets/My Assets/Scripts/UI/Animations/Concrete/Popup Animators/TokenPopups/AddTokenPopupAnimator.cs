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

	[SerializeField] private GameObject loadingLine;
	[SerializeField] private GameObject checkMarkIcon;

	//public bool ValidAddress { get; set; }

	//public bool ValidSymbol { get; set; }

	//public bool ValidDecimals { get; set; }

	//public bool CustomSymbol { get; set; }

	//private bool realTokenAddress;

	//public bool RealTokenAddress
	//{
	//	get { return realTokenAddress; }
	//	set
	//	{
	//		realTokenAddress = value;

	//		checkMarkIcon.AnimateGraphicAndScale(realTokenAddress ? 1f : 0f, realTokenAddress ? 1f : 0f, 0.1f);
	//		noTokenFound.AnimateGraphicAndScale(realTokenAddress ? 0f : 1f, realTokenAddress ? 0f : 1f, 0.15f);
	//		symbolSection.AnimateScaleX(CustomSymbol ? 1f : 0f, 0.15f);
	//		decimalSection.AnimateScaleX(CustomSymbol ? 1f : 0f, 0.15f);
	//		tokenSection.AnimateScaleX(CustomSymbol ? 0f : 1f, 0.15f);
	//	}
	//}

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

	public void AnimateFieldError(TMP_InputField inputField, bool animateIn)
	{
		inputField.transform.GetChild(1).gameObject.AnimateGraphicAndScale(animateIn ? 1f : 0f, animateIn ? 1f : 0f, 0.1f);
	}

	public void AnimateLoadingLine(bool animateIn)
	{
		if (animateIn)
			loadingLine.SetActive(true);

		loadingLine.AnimateScaleY(animateIn ? 1f : 0f, 0.1f, () => { if (!animateIn) loadingLine.SetActive(false); });
		noTokenFound.AnimateGraphicAndScale(animateIn ? 0f : 1f, animateIn ? 0f : 1f, 0.1f);
	}

    private void OnStatusChanged(AddTokenPopup.Status tokenPopupStatus)
    {
        //addTokenPopupAnimator.ValidAddress = string.IsNullOrEmpty(updatedAddress) || AddressUtils.IsValidEthereumAddress(updatedAddress);
        //addTokenPopupAnimator.AnimateLoadingLine(addTokenPopupAnimator.ValidAddress && !string.IsNullOrEmpty(updatedAddress));

        //addTokenPopupAnimator.AnimateFieldError(addressField, !addTokenPopupAnimator.ValidAddress);

        //okButton.interactable = addTokenPopupAnimator.RealTokenAddress;
    }
}
