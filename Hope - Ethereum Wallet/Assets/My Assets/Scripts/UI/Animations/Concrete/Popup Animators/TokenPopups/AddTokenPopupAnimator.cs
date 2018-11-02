using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
	private GameObject previousSection;

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
		previousStatus = AddTokenPopup.Status.NoTokenFound;
		previousSection = sections[0];
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
			previousSection.SetActive(false);

			switch (tokenPopupStatus)
			{
				case AddTokenPopup.Status.NoTokenFound:
				case AddTokenPopup.Status.TooManyTokensFound:
					sections[0].SetActive(true);
					previousSection = sections[0];
					break;
				case AddTokenPopup.Status.MultipleTokensFound:
					sections[1].SetActive(true);
					previousSection = sections[1];
					break;
				case AddTokenPopup.Status.InvalidToken:
					sections[2].SetActive(true);
					previousSection = sections[2];
					break;
				case AddTokenPopup.Status.ValidToken:
					sections[3].SetActive(true);
					previousSection = sections[3];
					break;
				case AddTokenPopup.Status.Loading:
					loadingIcon.SetActive(true);
					previousSection = loadingIcon;
					break;
			}
		}

		previousStatus = tokenPopupStatus;
	}
}
