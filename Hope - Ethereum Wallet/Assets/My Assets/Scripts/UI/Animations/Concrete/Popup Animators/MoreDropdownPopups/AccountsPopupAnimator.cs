using Nethereum.Signer;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountsPopupAnimator : UIAnimator
{
	public Action<int, int> AnimatePageChange { get; private set; }

	[SerializeField] private GameObject topSection;
	[SerializeField] private GameObject line;
	[SerializeField] private Transform addressSection;
	[SerializeField] private Transform pageSection;
	[SerializeField] private GameObject unlockButton;

	private void Awake() => AnimatePageChange = AnimateAddresses;

	protected override void AnimateUniqueElementsIn()
	{
		topSection.AnimateScaleX(1f, 0.175f);
		line.AnimateScaleX(1f, 0.2f);

		float duration = 0.2f;
		for (int i = 0; i < addressSection.childCount; i++)
		{
			addressSection.GetChild(i).gameObject.AnimateScaleX(1f, duration);
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

	private void AnimateAddresses(int firstAddressNumInList, int currentlySelectedAddress)
	{
		for (int i = 0; i < 5; i++)
			AnimateAddress(i, firstAddressNumInList, currentlySelectedAddress);
	}

	private void AnimateAddress(int i, int firstAddressNumInList, int currentlySelectedAddress)
	{
		GameObject addressObject = addressSection.transform.GetChild(i).gameObject;

		addressObject.AnimateScaleY(0f, 0.15f, () =>
		{
			addressObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = EthECKey.GenerateKey().GetPublicAddress();
			addressObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (firstAddressNumInList + i).ToString();
			SetAddressButtonInteractable(addressObject, currentlySelectedAddress != (firstAddressNumInList + i));
			addressObject.AnimateScaleY(1f, 0.15f);
		});
	}

	private void SetAddressButtonInteractable(GameObject addressObject, bool interactable)
	{
		addressObject.GetComponent<Button>().interactable = interactable;

		for (int i = 0; i < addressObject.transform.childCount; i++)
			addressObject.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color = interactable ? UIColors.LightGrey : UIColors.White;
	}
}
