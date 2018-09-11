using System;
using UnityEngine;

public class SettingsPopupAnimator : UIAnimator
{
	public Action<bool> CreateNewPassword, VerifyingPassword;

	[SerializeField] private GameObject settingsCategoriesParent;
	[SerializeField] private GameObject line;
	[SerializeField] private GameObject[] sections;

	[SerializeField] private GameObject nextButton;
	[SerializeField] private GameObject newPasswordInputField;
	[SerializeField] private GameObject confirmPasswordInputField;
	[SerializeField] private GameObject saveButton;
	[SerializeField] private GameObject loadingIcon;

	private CategoryButtons settingsCategories;

	private void Awake()
	{
		CreateNewPassword = AnimateNewPasswordVisuals;
		VerifyingPassword = AnimateVerifyingPasswordVisuals;

		settingsCategories = settingsCategoriesParent.GetComponent<CategoryButtons>();
		settingsCategories.OnButtonChanged += CategoryChanged;
	}

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

	private void AnimateNewPasswordVisuals(bool createNewPassword)
	{
		loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.15f);
		newPasswordInputField.AnimateScale(createNewPassword ? 1f : 0f, 0.2f);
		confirmPasswordInputField.AnimateScale(createNewPassword ? 1f : 0f, 0.25f);
		saveButton.AnimateScale(createNewPassword ? 1f : 0f, 0.3f);

		if (!createNewPassword)
			nextButton.AnimateGraphicAndScale(1f, 1f, 0.15f);
	}

	private void AnimateVerifyingPasswordVisuals(bool verifying)
	{
		loadingIcon.AnimateGraphicAndScale(verifying ? 1f : 0f, verifying ? 1f : 0f, 0.15f);
		nextButton.AnimateGraphicAndScale(verifying ? 0f : 1f, verifying ? 0f : 1f, 0.15f);
	}
}
