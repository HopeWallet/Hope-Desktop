using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactsPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject addContactButton;
	[SerializeField] private GameObject sortBySection;
	[SerializeField] private GameObject searchSection;
	[SerializeField] private GameObject contactsList;
	[SerializeField] private GameObject confirmButton;

	private Transform contactsTransform;

	public string SelectedContactName { get; private set; }

	public string SelectedContactAddress { get; private set; }

	/// <summary>
	/// Initializes the elements
	/// </summary>
	private void Awake()
	{
		contactsTransform = contactsList.transform.GetChild(0).GetChild(0);

		sortBySection.transform.GetChild(1).GetComponent<TMP_Dropdown>().onValueChanged.AddListener(ListOrderChanged);

		//SelectedContactName = "MY BOi JOEYYY";
		//SelectedContactAddress = "0xbF203720DaA26c88114273471cC5f3C83c7A0246";

		//SecurePlayerPrefs.SetString(ContactsPopup.PREF_NAME + 0, SelectedContactAddress);
		//SecurePlayerPrefs.SetString(SelectedContactAddress, SelectedContactName);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.2f,
			() => addContactButton.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => searchSection.AnimateScaleX(1f, 0.15f,
			() => confirmButton.AnimateGraphicAndScale(1f, 1f, 0.15f))));

		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateScaleX(1f, 0.15f,
			() => sortBySection.AnimateScaleX(1f, 0.15f,
			() => contactsList.AnimateScaleX(1f, 0.15f,
			() => AnimateContacts(0)))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		for (int i = 0; i < contactsTransform.childCount; i++)
			contactsTransform.GetChild(i).gameObject.AnimateScaleX(0f, 0.2f);

		contactsList.AnimateScaleX(0f, 0.15f,
			() => searchSection.AnimateScaleX(0f, 0.15f,
			() => addContactButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f))));
		confirmButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => sortBySection.AnimateScaleX(0f, 0.15f,
			() => title.AnimateScaleX(0f, 0.15f,
			() => dim.AnimateGraphic(0f, 0.15f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates each individual contact in the contactList	
	/// </summary>
	/// <param name="index"> The index of the contact in the list being animated </param>
	private void AnimateContacts(int index)
	{
		if (index == 6)
		{
			for (int i = 6; i < contactsTransform.childCount; i++)
				contactsTransform.GetChild(i).gameObject.transform.localScale = new Vector2(1f, 1f);

			FinishedAnimating();
		}
		else
			contactsTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.15f, () => AnimateContacts(++index));
	}
	private void ContactClicked(int index)
	{
		//Set the selected button interactable to false

		confirmButton.GetComponent<Button>().interactable = true;
	}

	/// <summary>
	/// List order has been changed
	/// </summary>
	/// <param name="value"> The value of the sorting type in the dropdown </param>
	private void ListOrderChanged(int value)
	{
	}

	private void ConfirmButtonClicked() => AnimateDisable();
}
