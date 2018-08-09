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
	[SerializeField] private GameObject loadingIcon;
	[SerializeField] private GameObject noTokenFoundSection;
	[SerializeField] private GameObject invalidTokenSection;
	[SerializeField] private GameObject validTokenSection;
	[SerializeField] private GameObject addTokenButton;

	private AddTokenPopup.Status previousStatus;

	//ADD THE ERROR AND CHECKMARK ICONS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
        GetComponent<AddTokenPopup>().OnStatusChanged += OnStatusChanged;
		previousStatus = AddTokenPopup.Status.NoTokenFound;
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
			() => noTokenFoundSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
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

		loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.15f);
		noTokenFoundSection.AnimateGraphicAndScale(0f, 0f, 0.15f);
		invalidTokenSection.AnimateGraphicAndScale(0f, 0f, 0.15f);
		validTokenSection.AnimateGraphicAndScale(0f, 0f, 0.15f);
	}

	/// <summary>
	/// Gets the current status of the AddTokenPopup and sets the proper sections to visible or not
	/// </summary>
	/// <param name="tokenPopupStatus"> The AddTokenPopup status </param>
	private void OnStatusChanged(AddTokenPopup.Status tokenPopupStatus)
    {
		if (tokenPopupStatus == previousStatus)
			return;

		switch (tokenPopupStatus)
		{
			case AddTokenPopup.Status.Loading:
				ChangeStatus(loading: true);
				break;
			case AddTokenPopup.Status.NoTokenFound:
				ChangeStatus(noTokenFound: true);
				break;
			case AddTokenPopup.Status.InvalidToken:
				ChangeStatus(invalidToken: true);
				break;
			case AddTokenPopup.Status.ValidToken:
				ChangeStatus(validToken: true);
				break;
		}

		previousStatus = tokenPopupStatus;
	}

	/// <summary>
	/// Changes the sections according to their booleans
	/// </summary>
	/// <param name="loading"> Whether the loading icon should appear </param>
	/// <param name="noTokenFound"> Whether the noTokenFoundSection should appear </param>
	/// <param name="invalidToken"> Whether the invalidTokenSection should appear </param>
	/// <param name="validToken"> Whether the validTokenSection should appear </param>
	private void ChangeStatus(bool loading = false, bool noTokenFound = false, bool invalidToken = false, bool validToken = false)
	{
		AnimateSection(loadingIcon, loading);
		AnimateSection(noTokenFoundSection, noTokenFound);
		AnimateSection(invalidTokenSection, invalidToken);
		AnimateSection(validTokenSection, validToken);
	}

	/// <summary>
	/// Animates a section in if the boolean states, or makes the section disappear otherwise
	/// </summary>
	/// <param name="section"> The section being modified </param>
	/// <param name="animateIn"> Whether animating section in or not </param>
	private void AnimateSection(GameObject section, bool animateIn)
	{
		if (animateIn)
		{
			if (section == loadingIcon) loadingIcon.SetActive(true);
			section.AnimateGraphicAndScale(1f, 1f, 0.1f);
		}
		else
		{
			if (section == loadingIcon)
			{
				loadingIcon.SetActive(false);
				loadingIcon.GetComponent<Image>().color = Color.clear;
			}
			section.transform.localScale = Vector2.zero;
		}
	}
}
