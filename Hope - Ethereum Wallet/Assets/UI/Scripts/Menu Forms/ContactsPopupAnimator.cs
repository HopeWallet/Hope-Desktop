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
	[SerializeField] private GameObject sortByDropdown;
	[SerializeField] private GameObject searchSection;
	[SerializeField] private GameObject contactsList;
	[SerializeField] private GameObject confirmButton;

	private List<GameObject> contacts;

	private Transform contactsTransform;

	public string selectedContactName { get; private set; }
	public string selectedContactAddress { get; private set; }

	/// <summary>
	/// Initializes the elements
	/// </summary>
	private void Awake()
	{
		contacts = new List<GameObject>();
		contactsTransform = contactsList.transform.GetChild(0).GetChild(0);

		for (int i = 0; i < contactsTransform.childCount; i++)
		{
			contacts.Add(contactsTransform.GetChild(i).gameObject);
			contactsTransform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => ContactClicked(i));
		}
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
			() => sortByDropdown.AnimateScaleX(1f, 0.15f,
			() => contactsList.AnimateScaleX(1f, 0.15f,
			() => AnimateContacts(0)))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		FinishedAnimating();
	}

	/// <summary>
	/// Animates each individual contact in the contactList	
	/// </summary>
	/// <param name="index"> The index of the contact in the list being animated </param>
	private void AnimateContacts(int index)
	{
		contacts[index].SetActive(true);

		if (index == contactsTransform.childCount - 1)
			contacts[index].AnimateScaleX(1f, 0.15f, FinishedAnimating);
		else
			contacts[index].AnimateScaleX(1f, 0.15f, () => AnimateContacts(++index));
	}
	private void ContactClicked(int index)
	{
		//Set the selected button interactable to false

		confirmButton.GetComponent<Button>().interactable = true;
	}

	private void EditContactClicked()
	{

	}

	private void ConfirmButtonClicked() => AnimateDisable();
}
