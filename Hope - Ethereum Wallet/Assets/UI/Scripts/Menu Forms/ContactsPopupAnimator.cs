using System.Collections.Generic;
using UnityEngine;

public class ContactsPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject contactNameSection;
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject contactsList;

	private List<GameObject> contacts;
	private Transform contactsTransform;

	/// <summary>
	/// Initializes the elements
	/// </summary>
	private void Awake()
	{
		contacts = new List<GameObject>();
		contactsTransform = contactsList.transform.GetChild(0).GetChild(0);

		for (int i = 0; i < contactsTransform.childCount; i++)
			contacts.Add(contactsTransform.GetChild(i).gameObject);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.15f);
		form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => contactsList.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => AnimateContacts(0)));
		title.AnimateGraphicAndScale(0.85f, 1f, 0.15f,
			() => contactNameSection.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => addressSection.AnimateGraphicAndScale(1f, 1f, 0.15f)));
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
}
