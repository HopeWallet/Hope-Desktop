using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the ContactsPopup
/// </summary>
public sealed class ContactsPopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject addContactButton;
	[SerializeField] private GameObject sortBySection;
	[SerializeField] private GameObject searchInputField;
	[SerializeField] private GameObject line;
	[SerializeField] private GameObject contactsList;
	[SerializeField] private GameObject confirmButton;
	[SerializeField] private Scrollbar scrollbar;

	private Transform contactsTransform;

	/// <summary>
	/// Initializes the elements
	/// </summary>
	private void Start() => contactsTransform = contactsList.transform.GetChild(0).GetChild(0);

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		scrollbar.value = 1f;
		addContactButton.AnimateGraphicAndScale(1f, 1f, 0.15f);
		sortBySection.AnimateScaleX(1f, 0.2f);
		searchInputField.AnimateScaleX(1f, 0.2f, () => AnimateContacts(0));
		line.AnimateScaleX(1f, 0.25f);
		confirmButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
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
}
