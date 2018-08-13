using UnityEngine;
using UnityEngine.UI;

public class ContactsPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject addContactButton;
	[SerializeField] private GameObject sortBySection;
	[SerializeField] private GameObject searchSection;
	[SerializeField] private GameObject contactsList;
	[SerializeField] private GameObject confirmButton;

	private Transform contactsTransform;

	/// <summary>
	/// Initializes the elements
	/// </summary>
	private void Start() => contactsTransform = contactsList.transform.GetChild(0).GetChild(0);

	protected override void AnimateUniqueElementsIn()
	{
		throw new System.NotImplementedException();
	}

	protected override void AnimateUniqueElementsOut()
	{
		throw new System.NotImplementedException();
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
