using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountsPopup : OkCancelPopupComponent<AccountsPopup>
{
	[SerializeField] private GeneralRadioButtons addressesSection;
	[SerializeField] private Button previousPageButton, nextPageButton;
	[SerializeField] private TextMeshProUGUI pageNumText;

	private int pageNum = 1, firstAddressNumInList = 1;

	protected override void Awake()
	{
		base.Awake();

		previousPageButton.onClick.AddListener(() => PageChanged(false));
		nextPageButton.onClick.AddListener(() => PageChanged(true));
	}

	private void OnDestroy() => MoreDropdown.PopupClosed?.Invoke();

	private void PageChanged(bool nextPage)
	{
		pageNum = nextPage ? ++pageNum : --pageNum;
		firstAddressNumInList = nextPage ? firstAddressNumInList += 5 : firstAddressNumInList -= 5;
		pageNumText.text = pageNum.ToString();

		for (int i = 0; i < 5; i++)
			addressesSection.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = (firstAddressNumInList + i).ToString();

		nextPageButton.interactable = pageNum != 10;
		previousPageButton.interactable = pageNum != 1;
	}
}