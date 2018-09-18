using UnityEngine;

/// <summary>
/// The animator class of the OpenTrezorWalletMenu
/// </summary>
public sealed class OpenTrezorWalletMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject enterPINText;
	[SerializeField] private GameObject subText;
	[SerializeField] private Transform keyPadButtons;
	[SerializeField] private GameObject passcodeInputField;
	[SerializeField] private GameObject nextButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateIn()
	{
		base.AnimateIn();

		enterPINText.AnimateGraphicAndScale(1f, 1f, 0.25f);
		subText.AnimateGraphicAndScale(1f, 1f, 0.275f);

		float duration = 0.3f;
		for (int i = 0; i < 9; i++)
		{
			keyPadButtons.GetChild(i).gameObject.AnimateGraphicAndScale(1f, 1f, duration);

			if (i == 2 || i == 5)
				duration += 0.025f;
		}

		passcodeInputField.AnimateScaleX(1f, 0.375f);
		nextButton.AnimateGraphicAndScale(1f, 1f, 0.4f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		base.AnimateOut();

		enterPINText.AnimateGraphicAndScale(0f, 0f, 0.3f);
		subText.AnimateGraphicAndScale(0f, 0f, 0.3f);

		for (int i = 0; i < 9; i++)
			keyPadButtons.GetChild(i).gameObject.AnimateGraphicAndScale(0f, 0f, 0.3f);

		passcodeInputField.AnimateScaleX(0f, 0.3f);
		nextButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
	}
}