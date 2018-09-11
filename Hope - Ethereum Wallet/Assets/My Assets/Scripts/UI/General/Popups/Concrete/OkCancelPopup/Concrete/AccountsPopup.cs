using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountsPopup : OkCancelPopupComponent<AccountsPopup>
{
	[SerializeField] private GeneralRadioButtons addressesCategories;
	[SerializeField] private Transform addressesSection;
	[SerializeField] private Button previousPageButton, nextPageButton;
	[SerializeField] private TextMeshProUGUI pageNumText;

	private int pageNum = 1, firstAddressNumInList = 1, currentlySelectedAddress = 1, currentlyActivatedAddress = 1;

	protected override void Awake()
	{
		base.Awake();

		addressesCategories.OnButtonChanged += AddressCategoryChanged;
		previousPageButton.onClick.AddListener(() => PageChanged(false));
		nextPageButton.onClick.AddListener(() => PageChanged(true));

		for (int i = 0; i < 5; i++)
			SetButtonListener(i);
	}

	private void SetButtonListener(int num) => addressesSection.GetChild(num).GetComponent<Button>().onClick.AddListener(() => AddressClicked(num));

	private void AddressClicked(int num)
	{
		SetAddressInteractable(addressesSection.GetChild(currentlySelectedAddress % 5), true);

		Transform selectedAddressTransform = addressesSection.GetChild(num);

		SetAddressInteractable(selectedAddressTransform, false);
		currentlySelectedAddress = int.Parse(selectedAddressTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text);

		okButton.interactable = currentlyActivatedAddress != currentlySelectedAddress;
	}

	private void OnDestroy() => MoreDropdown.PopupClosed?.Invoke();

	private void PageChanged(bool movingForward)
	{
		if (pageNum == 1 && !movingForward)
			pageNum = 10;
		else if (pageNum == 10 && movingForward)
			pageNum = 1;
		else
			pageNum = movingForward ? ++pageNum : --pageNum;

		firstAddressNumInList = (pageNum * 5) - 4;
		pageNumText.text = pageNum.ToString();

		for (int i = 0; i < 5; i++)
		{
			Transform addressTransform = addressesSection.GetChild(i);

			addressTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (firstAddressNumInList + i).ToString();

			if (currentlySelectedAddress == (firstAddressNumInList + i))
				SetAddressInteractable(addressTransform, false);
			else
				SetAddressInteractable(addressTransform, true);
		}
	}

	private void AddressCategoryChanged(int num)
	{
		if (num == 0)
		{
			//Default addresses
		}
		else
		{
			//Ledger legacy addresses
		}
	}

	private void SetAddressInteractable(Transform addressTransform, bool interactable)
	{
		addressTransform.GetComponent<Button>().interactable = interactable;

		for (int i = 0; i < addressTransform.childCount; i++)
			addressTransform.GetChild(i).gameObject.AnimateColor(interactable ? UIColors.LightGrey : UIColors.White, 0.15f);
	}
}