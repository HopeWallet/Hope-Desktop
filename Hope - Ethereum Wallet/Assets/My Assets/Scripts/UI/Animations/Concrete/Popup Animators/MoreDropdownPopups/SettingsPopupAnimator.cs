using System;
using UnityEngine;

public class SettingsPopupAnimator : UIAnimator
{
	public Action<bool> VerifyingPassword;
	public Action EditWallet;

	[SerializeField] private GameObject settingsCategoriesParent;
	[SerializeField] private GameObject line;
	[SerializeField] private GameObject[] sections;

	[SerializeField] private GameObject editWalletButton,
										currentPasswordField,
										walletNameField,
										newPasswordField,
										confirmPasswordField,
										deleteButton,
										saveButton,
										loadingIcon;

	private CategoryButtons settingsCategories;

	/// <summary>
	/// Sets the necessary values
	/// </summary>
	private void Awake()
	{
		EditWallet = AnimateOtherFields;
		VerifyingPassword = AnimateLoadingIcon;

		settingsCategories = settingsCategoriesParent.GetComponent<CategoryButtons>();
		settingsCategories.OnButtonChanged += CategoryChanged;
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		settingsCategoriesParent.AnimateScaleY(1f, 0.25f);
		line.AnimateScaleX(1f, 0.25f);
		sections[0].AnimateScale(1f, 0.3f, FinishedAnimating);
	}

	private void CategoryChanged(int categoryNum)
	{
		sections[settingsCategories.previouslySelectedButton].AnimateScale(0f, 0.15f, () => sections[categoryNum].AnimateScale(1f, 0.15f));
	}

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

	private void AnimateLoadingIcon(bool verifying)
	{
		if (verifying)
			loadingIcon.SetActive(true);

		loadingIcon.AnimateGraphicAndScale(verifying ? 1f : 0f, verifying ? 1f : 0f, 0.15f, () => { if (!verifying) loadingIcon.SetActive(false); });
		editWalletButton.AnimateGraphicAndScale(verifying ? 0f : 1f, verifying ? 0f : 1f, 0.15f);
	}
}
