using System;
using UnityEngine;

/// <summary>
/// The animator class of the SettingsPopup
/// </summary>
public sealed class SettingsPopupAnimator : PopupAnimator
{
	public Action<GameObject, GameObject, bool> VerifyingPassword;
	public Action<GameObject, GameObject, GameObject> PasswordVerificationFinished;

	[SerializeField] private GameObject settingsCategoriesParent;
	[SerializeField] private GameObject line1, line2, line3;
	[SerializeField] private GameObject[] sections;

	private IconButtons settingsCategories;

	/// <summary>
	/// Sets the necessary values
	/// </summary>
	private void Awake()
	{
		PasswordVerificationFinished = VerificationFinished;
		VerifyingPassword = AnimateLoadingIcon;

		settingsCategories = settingsCategoriesParent.GetComponent<IconButtons>();
		settingsCategories.OnButtonChanged += CategoryChanged;
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		line1.AnimateScaleX(1f, 0.25f);
		line2.AnimateScaleX(1f, 0.25f);
		line3.AnimateScaleX(1f, 0.25f);
		settingsCategoriesParent.AnimateScaleX(1f, 0.25f);
		sections[0].AnimateScale(1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the current category out, and the new category in
	/// </summary>
	/// <param name="categoryNum"> The number of the category in the array </param>
	private void CategoryChanged(int categoryNum)
	{
		sections[settingsCategories.PreviouslyActiveButton].AnimateScale(0f, 0.15f, () => sections[categoryNum].AnimateScale(1f, 0.15f));

		if (categoryNum == 4 || categoryNum == 5 || categoryNum == 6)
		{
			HopeInputField currentPasswordField = sections[categoryNum].transform.GetChild(0).GetComponent<HopeInputField>();

			if (currentPasswordField.Text == string.Empty)
				currentPasswordField.InputFieldBase.ActivateInputField();
		}
	}

	/// <summary>
	/// Animates an icon in and out of view
	/// </summary>
	/// <param name="button"> The given button to be animated back in </param>
	/// <param name="loadingIcon"> The loading icon being animated out </param>
	/// <param name="finishIcon"> The finish icon to be shown </param>
	public void VerificationFinished(GameObject button, GameObject loadingIcon, GameObject finishIcon)
	{
		VerifyingPassword.Invoke(button, loadingIcon, false);

		finishIcon.transform.localScale = new Vector3(0, 0, 1);

		finishIcon.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => CoroutineUtils.ExecuteAfterWait(0.6f, () => { if (finishIcon != null) finishIcon.AnimateGraphic(0f, 0.25f); }));
	}

	/// <summary>
	/// Animates the loading icon in or out of view
	/// </summary>
	/// <param name="button"> The button to be animated </param>
	/// <param name="loadingIcon"> The loading icon to be animated </param>
	/// <param name="verifying"> Whether the password is currently being verified or not </param>
	private void AnimateLoadingIcon(GameObject button, GameObject loadingIcon, bool verifying)
	{
		if (verifying)
			loadingIcon.SetActive(true);

		loadingIcon.AnimateGraphicAndScale(verifying ? 1f : 0f, verifying ? 1f : 0f, 0.15f, () => { if (!verifying) loadingIcon.SetActive(false); });
		button.AnimateGraphicAndScale(verifying ? 0f : 1f, verifying ? 0f : 1f, 0.15f);
	}
}
