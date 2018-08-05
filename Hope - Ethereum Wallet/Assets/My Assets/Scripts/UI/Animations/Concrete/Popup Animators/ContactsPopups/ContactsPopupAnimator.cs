using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactsPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
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
	private void Start()
	{
		contactsTransform = contactsList.transform.GetChild(0).GetChild(0);

		sortBySection.transform.GetChild(1).GetComponent<TMP_Dropdown>().onValueChanged.AddListener(ListOrderChanged);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(0.25f, 0.2f);
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
			() => { blur.AnimateMaterialBlur(-0.25f, 0.15f); dim.AnimateGraphic(0f, 0.15f, FinishedAnimating); })));
	}

	/// <summary>
	/// Animates each individual contact in the contactList	
	/// </summary>
	/// <param name="index"> The index of the contact in the list being animated </param>
	private void AnimateContacts(int index)
	{
		if (contactsTransform.childCount == 0)
		{
			FinishedAnimating();
			return;
		}

		if (index == 6)
		{
			for (int i = index; i < contactsTransform.childCount; i++)
				contactsTransform.GetChild(i).gameObject.transform.localScale = new Vector2(1f, 1f);

			FinishedAnimating();
		}
		else if (index == contactsTransform.childCount - 1)
			contactsTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.1f, FinishedAnimating);
		else
			contactsTransform.GetChild(index).gameObject.AnimateScaleX(1f, 0.1f, () => AnimateContacts(++index));
	}

	/// <summary>
	/// List order has been changed
	/// </summary>
	/// <param name="value"> The value of the sorting type in the dropdown </param>
	private void ListOrderChanged(int value)
	{
		if (value == 0)
		{
			//sort by oldest
		}

		else if (value == 1)
		{
			//Sort by newest
		}

		else
		{
			//Sort by alphabetical
		}
	}
}
