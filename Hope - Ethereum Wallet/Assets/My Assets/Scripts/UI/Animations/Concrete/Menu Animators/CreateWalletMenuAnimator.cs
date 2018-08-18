using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which animates the CreateWalletMenu.
/// </summary>
public class CreateWalletMenuAnimator : UIAnimator
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
		FinishedAnimating();
		//walletNameField.AnimateScaleX(1f, 0.1f);
		//password1Field.AnimateScaleX(1f, 0.2f);
		//password2Field.AnimateScaleX(1f, 0.2f);
		//nextButton.AnimateGraphicAndScale(1f, 1f, 0.25f, FinishedAnimating);
	}

	/// <summary>
	/// Resets the unique elements of the form back to the starting positions
	/// </summary>
	protected override void ResetElementValues()
	{
		FinishedAnimating();

		//walletNameField.SetScale(new Vector2(0f, 1f));
		//password1Field.SetScale(new Vector2(0f, 1f));
		//password2Field.SetScale(new Vector2(0f, 1f));
		//nextButton.SetGraphicAndScale(Vector2.zero);
	}
}
