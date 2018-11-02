using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the AddTokenPopup
/// </summary>
public sealed class AddTokenPopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject addressInputField;
	[SerializeField] private GameObject loadingIcon;
	[SerializeField] private GameObject[] sections;
	[SerializeField] private GameObject addTokenButton;

	private Dictionary<AddTokenPopup.Status, Tuple<string, string>> textInfo;

	private AddTokenPopup.Status previousStatus;

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
		previousStatus = AddTokenPopup.Status.NoTokenFound;
		GetComponent<AddTokenPopup>().OnStatusChanged += OnStatusChanged;

		textInfo = new Dictionary<AddTokenPopup.Status, Tuple<string, string>>();
		textInfo.Add(AddTokenPopup.Status.NoTokenFound, Tuple.Create("No token found.", "Please enter a valid ERC20 token name/symbol/address..."));
		textInfo.Add(AddTokenPopup.Status.TooManyTokensFound, Tuple.Create("Too many results.", "Please narrow your search to something more specific..."));
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		addressInputField.GetComponent<HopeInputField>().InputFieldBase.ActivateInputField();
		addressInputField.AnimateScaleX(1f, 0.2f);
		sections[0].AnimateGraphicAndScale(1f, 1f, 0.25f);
		addTokenButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Gets the current status of the AddTokenPopup and sets the proper sections to visible or not
	/// </summary>
	/// <param name="tokenPopupStatus"> The AddTokenPopup status </param>
	private void OnStatusChanged(AddTokenPopup.Status tokenPopupStatus)
	{
		tokenPopupStatus.Log();

		if (tokenPopupStatus == previousStatus)
			return;

		if (tokenPopupStatus == AddTokenPopup.Status.NoTokenFound || tokenPopupStatus == AddTokenPopup.Status.TooManyTokensFound)
		{
			sections[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = textInfo[tokenPopupStatus].Item1;
			sections[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textInfo[tokenPopupStatus].Item2;
		}

		if (!(previousStatus == AddTokenPopup.Status.NoTokenFound && tokenPopupStatus == AddTokenPopup.Status.TooManyTokensFound)
			&& !(previousStatus == AddTokenPopup.Status.TooManyTokensFound && tokenPopupStatus == AddTokenPopup.Status.NoTokenFound))
		{
			switch (tokenPopupStatus)
			{
				case AddTokenPopup.Status.NoTokenFound:
				case AddTokenPopup.Status.TooManyTokensFound:
					ChangeStatus(textSection: true);
					break;
				case AddTokenPopup.Status.MultipleTokensFound:
					ChangeStatus(multipleTokensFound: true);
					break;
				case AddTokenPopup.Status.InvalidToken:
					ChangeStatus(invalidToken: true);
					break;
				case AddTokenPopup.Status.ValidToken:
					ChangeStatus(validToken: true);
					break;
				case AddTokenPopup.Status.Loading:
					ChangeStatus(loading: true);
					break;
			}
		}

		previousStatus = tokenPopupStatus;
	}

	/// <summary>
	/// Changes the sections according to their booleans
	/// </summary>
	/// <param name="textSection"> Whether the noTokenFoundSection should appear </param>
	/// <param name="multipleTokensFound"> Whether the tokenList should appear </param>
	/// <param name="invalidToken"> Whether the invalidTokenSection should appear </param>
	/// <param name="validToken"> Whether the validTokenSection should appear </param>
	/// <param name="loading"> Whether the loading icon should appear </param>
	private void ChangeStatus(bool textSection = false, bool multipleTokensFound = false, bool invalidToken = false, bool validToken = false, bool loading = false)
	{
		AnimateSection(sections[0], textSection);
		AnimateSection(sections[1], multipleTokensFound);
		AnimateSection(sections[2], invalidToken);
		AnimateSection(sections[3], validToken);
		AnimateSection(loadingIcon, loading);
	}

	/// <summary>
	/// Animates a section in if the boolean states, or makes the section disappear otherwise
	/// </summary>
	/// <param name="section"> The section being modified </param>
	/// <param name="animateIn"> Whether animating section in or not </param>
	private void AnimateSection(GameObject section, bool animateIn)
	{
		if (section == loadingIcon && animateIn)
			loadingIcon.SetActive(true);

		section.AnimateGraphicAndScale(animateIn ? 1f : 0f, animateIn ? 1f : 0f, animateIn ? 0.2f : 0.1f, () => { if (section == loadingIcon) loadingIcon.SetActive(false); });
	}
}
