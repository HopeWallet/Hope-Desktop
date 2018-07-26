using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

public class ContactButton : InfoButton<ContactButton, TradableAsset>
{

	public TextMeshProUGUI contactName, contactAddress;
	public Button editButton, deleteButton;

	private PopupManager popupManager;

	[Inject]
	public void Construct(PopupManager popupManager)
	{
		this.popupManager = popupManager;
	}

	protected override void OnAwake()
	{
		Button.onClick.AddListener(ContactClicked);
		editButton.onClick.AddListener(EditContact);
	}

	private void ContactClicked()
	{
		Button.interactable = !Button.interactable;
	}

	private void EditContact()
	{
		popupManager.GetPopup<AddOrEditContactPopup>(true).SetPopupLayout(false, contactName.text, contactAddress.text);
		//popupManager.GetPopup<AddOrEditContactPopup>(true).SetDictionary(contacts);
	}

	private void DeleteContact()
	{

	}
}
