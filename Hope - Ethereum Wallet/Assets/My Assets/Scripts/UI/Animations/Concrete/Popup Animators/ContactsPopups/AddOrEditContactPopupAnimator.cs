﻿using UnityEngine;
using UnityEngine.UI;
using Hope.Utils.Ethereum;
using Zenject;

/// <summary>
/// The animator class of the AddOrEditContactPopup
/// </summary>
public sealed class AddOrEditContactPopupAnimator : PopupAnimator
{
	[SerializeField] private HopeInputField nameInputField;
	[SerializeField] private HopeInputField addressInputField;
	[SerializeField] private GameObject addContactButton;
	[SerializeField] private GameObject confirmButton;

	private AddOrEditContactPopup addOrEditContactPopup;
	public ContactsManager contactsManager;
	private RestrictedAddressManager restrictedAddressManager;

	public string PreviousAddress { get; set; }

    public bool AddingContact { get; set; }

	/// <summary>
	/// Sets the required dependency
	/// </summary>
	/// <param name="restrictedAddressManager"> The active RestrictedAddressManager </param>
	[Inject]
	public void Construct(RestrictedAddressManager restrictedAddressManager)
	{
		this.restrictedAddressManager = restrictedAddressManager;
	}

	/// <summary>
	/// Initializes the elements
	/// </summary>
	private void Awake()
	{
		addOrEditContactPopup = transform.GetComponent<AddOrEditContactPopup>();

		nameInputField.OnInputUpdated += ContactNameChanged;
		addressInputField.OnInputUpdated += AddressChanged;
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		nameInputField.InputFieldBase.ActivateInputField();
		nameInputField.gameObject.AnimateScaleX(1f, 0.2f);
		addressInputField.gameObject.AnimateScaleX(1f, 0.25f);
		AnimateMainButton(true);
	}

	/// <summary>
	/// Animates the main button, depending on the boolean addingContact
	/// </summary>
	/// <param name="animateIn"> Checks if animating the button in or out </param>
	private void AnimateMainButton(bool animateIn)
	{
		float endValue = animateIn ? 1f : 0f;

		if (AddingContact)
			addContactButton.AnimateGraphicAndScale(endValue, endValue, animateIn ? 0.3f : 0.2f);
		else
			confirmButton.AnimateGraphicAndScale(endValue, endValue, animateIn ? 0.3f : 0.2f);

		if (animateIn) FinishedAnimating();
	}

	/// <summary>
	/// Checks if the name hasn't already been taken up by another contact
	/// </summary>
	/// <param name="name"> The current string in the name input field </param>
	private void ContactNameChanged(string name)
	{
		nameInputField.Error = string.IsNullOrEmpty(nameInputField.Text.Trim());

		SetMainButtonInteractable();
	}

	/// <summary>
	/// Checks to see if the text is a valid ethereum address
	/// </summary>
	/// <param name="address"> The current string in the address input field </param>
	private void AddressChanged(string address)
	{
		string updatedAddress = addressInputField.Text.ToLower();

		bool realEthereumAddress = AddressUtils.IsValidEthereumAddress(updatedAddress);
		bool overridingOtherContactAddresses = contactsManager.ContactList.Contains(updatedAddress) && (!AddingContact ? updatedAddress != PreviousAddress : true);
		bool isRestrictedAddress = restrictedAddressManager.IsRestrictedAddress(updatedAddress);

		addressInputField.Error = !realEthereumAddress || overridingOtherContactAddresses || isRestrictedAddress;

		if (!realEthereumAddress)
			addressInputField.errorMessage.text = "Invalid address";
		else if (overridingOtherContactAddresses)
			addressInputField.errorMessage.text = "Address in use";
		else if (isRestrictedAddress)
			addressInputField.errorMessage.text = "Restricted address";

		SetMainButtonInteractable();
	}

	/// <summary>
	/// Sets the main button to interactable depending on if the inputs are not empty, and are valid
	/// </summary>
	private void SetMainButtonInteractable()
	{
		bool validInputs = !nameInputField.Error && !addressInputField.Error;

		if (AddingContact)
			addContactButton.GetComponent<Button>().interactable = validInputs;
		else
			confirmButton.GetComponent<Button>().interactable = validInputs;
	}
}
