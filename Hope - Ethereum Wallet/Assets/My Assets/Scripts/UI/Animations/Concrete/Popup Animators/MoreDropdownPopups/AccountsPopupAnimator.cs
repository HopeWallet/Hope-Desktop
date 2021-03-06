﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the AccountsPopup
/// </summary>
public sealed class AccountsPopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject addressCategories;
	[SerializeField] private GameObject line;
	[SerializeField] private Transform addressSection;
	[SerializeField] private Transform pageSection;
	[SerializeField] private GameObject unlockButton;

    private void Awake() => GetComponent<AccountsPopup>().OnPageChanged += AnimateAddresses;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		addressCategories.AnimateScaleX(1f, 0.175f);
		line.AnimateScaleX(1f, 0.2f);

		float duration = 0.2f;
		for (int i = 0; i < addressSection.childCount; i++)
		{
			addressSection.GetChild(i).gameObject.AnimateScale(1f, duration);
			duration += 0.1f;
		}

		duration = 0.24f;
		for (int i = 0; i < pageSection.childCount; i++)
		{
			pageSection.GetChild(i).gameObject.AnimateGraphicAndScale(1f, 1f, duration);
			duration += 0.15f;
		}

		unlockButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the addresses
	/// </summary>
	/// <param name="addresses"> The array of address strings being set </param>
	/// <param name="firstAddressNumInList"> The number of the first address </param>
	/// <param name="currentlySelectedAddress"> The number of the currently selected address </param>
	private void AnimateAddresses(string[] addresses, int firstAddressNumInList, int currentlySelectedAddress)
	{
		for (int i = 0; i < 5; i++)
			AnimateAddress(addresses[i], i, firstAddressNumInList, currentlySelectedAddress);
	}

	/// <summary>
	/// Animates the given address object
	/// </summary>
	/// <param name="address"> The address string being set </param>
	/// <param name="index"> The address index </param>
	/// <param name="firstAddressNumInList"> The number of the first address </param>
	/// <param name="currentlySelectedAddress"> The number of the currently selected address </param>
	private void AnimateAddress(string address, int index, int firstAddressNumInList, int currentlySelectedAddress)
	{
		GameObject addressObject = addressSection.transform.GetChild(index).gameObject;

		addressObject.AnimateScaleY(0f, 0.12f, () =>
		{
			addressObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = address;
			addressObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (firstAddressNumInList + index).ToString();
			SetAddressButtonInteractable(addressObject, currentlySelectedAddress != (firstAddressNumInList + index));
			addressObject.AnimateScaleY(1f, 0.12f);
		});
	}

	/// <summary>
	/// Sets the given address button to interactable or not
	/// </summary>
	/// <param name="addressObject"> The address object </param>
	/// <param name="interactable"> Whether it should be interactable or not </param>
	private void SetAddressButtonInteractable(GameObject addressObject, bool interactable)
	{
		addressObject.GetComponent<Button>().interactable = interactable;

		for (int i = 0; i < addressObject.transform.childCount; i++)
			addressObject.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color = interactable ? UIColors.LightGrey : UIColors.White;
	}
}
