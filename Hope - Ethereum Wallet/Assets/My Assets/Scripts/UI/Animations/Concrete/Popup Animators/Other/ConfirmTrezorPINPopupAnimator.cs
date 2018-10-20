using UnityEngine;

/// <summary>
/// The animator class of the ConfirmTrezorPINPopup
/// </summary>
public class ConfirmTrezorPINPopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject enterPINText;
	[SerializeField] private GameObject subText;
	[SerializeField] private Transform keyPadButtons;
	[SerializeField] private GameObject passcodeInputField;
	[SerializeField] private GameObject confirmButton;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
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
		confirmButton.AnimateGraphicAndScale(1f, 1f, 0.4f, FinishedAnimating);
	}
}
