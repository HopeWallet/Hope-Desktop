using UnityEngine;

/// <summary>
/// The animator class of the CreateWalletMenu
/// </summary>
public sealed class CreateWalletMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject walletNameField;
	[SerializeField] private GameObject password1Field;
	[SerializeField] private GameObject password2Field;
	[SerializeField] private GameObject nextButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		walletNameField.GetComponent<HopeInputField>().InputFieldBase.ActivateInputField();
		walletNameField.AnimateScale(1f, 0.25f);
		password1Field.AnimateScale(1f, 0.3f);
		password2Field.AnimateScale(1f, 0.35f);
		nextButton.AnimateGraphicAndScale(1f, 1f, 0.4f, FinishedAnimating);
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		walletNameField.AnimateScale(0f, 0.3f);
		password1Field.AnimateScale(0f, 0.3f);
		password2Field.AnimateScale(0f, 0.3f);
		nextButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
	}
}
