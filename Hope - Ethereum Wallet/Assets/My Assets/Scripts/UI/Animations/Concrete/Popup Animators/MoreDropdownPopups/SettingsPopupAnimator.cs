using System;
using TMPro;
using UnityEngine;

/// <summary>
/// The animator class of the SettingsPopup
/// </summary>
public sealed class SettingsPopupAnimator : PopupAnimator
{
	public Action<bool> VerifyingPassword;
	public Action EditWallet;

	[SerializeField] private GameObject settingsCategoriesParent;
	[SerializeField] private GameObject line1;
	[SerializeField] private GameObject line2;
	[SerializeField] private GameObject[] sections;

	[SerializeField] private GameObject editWalletButton,
										currentPasswordField,
										walletNameField,
										newPasswordField,
										confirmPasswordField,
										deleteButton,
										saveButton,
										loadingIcon;

	private IconButtons settingsCategories;

	/// <summary>
	/// Sets the necessary values
	/// </summary>
	private void Awake()
	{
		EditWallet = AnimateOtherFields;
		VerifyingPassword = AnimateLoadingIcon;

		settingsCategories = settingsCategoriesParent.GetComponent<IconButtons>();
		settingsCategories.OnButtonChanged += CategoryChanged;
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		settingsCategoriesParent.AnimateScaleY(1f, 0.25f);
		line1.AnimateScaleX(1f, 0.25f);
		line2.AnimateScaleX(1f, 0.25f);
		sections[0].AnimateScale(1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the current category out, and the new category in
	/// </summary>
	/// <param name="categoryNum"> The number of the category in the array</param>
	private void CategoryChanged(int categoryNum)
	{
		sections[settingsCategories.previouslySelectedButton].AnimateScale(0f, 0.15f, () => sections[categoryNum].AnimateScale(1f, 0.15f));

		if (categoryNum == 2)
			currentPasswordField.GetComponent<HopeInputField>().InputFieldBase.ActivateInputField();
	}

	/// <summary>
	/// Animates the other input fields when user has input the correct password in the wallet section
	/// </summary>
	private void AnimateOtherFields()
	{
		currentPasswordField.AnimateScale(0f, 0.15f);
		loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.2f, () =>
		{
			loadingIcon.SetActive(false);
			walletNameField.AnimateScale(1f, 0.15f);
			newPasswordField.AnimateScale(1f, 0.2f);
			confirmPasswordField.AnimateScale(1f, 0.25f);
			deleteButton.AnimateScale(1f, 0.3f);
			saveButton.AnimateScale(1f, 0.3f);
		});
	}

	/// <summary>
	/// Animates the loading icon in or out of view
	/// </summary>
	/// <param name="verifying"> Whether the password is currently being checked or not </param>
	private void AnimateLoadingIcon(bool verifying)
	{
		if (verifying)
			loadingIcon.SetActive(true);

		loadingIcon.AnimateGraphicAndScale(verifying ? 1f : 0f, verifying ? 1f : 0f, 0.15f, () => { if (!verifying) loadingIcon.SetActive(false); });
		editWalletButton.AnimateGraphicAndScale(verifying ? 0f : 1f, verifying ? 0f : 1f, 0.15f);
	}
}
