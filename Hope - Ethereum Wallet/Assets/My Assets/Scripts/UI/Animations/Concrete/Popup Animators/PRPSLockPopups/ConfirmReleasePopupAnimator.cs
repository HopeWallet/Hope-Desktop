using UnityEngine;

/// <summary>
/// The animator class of the ConfirmReleasePopup
/// </summary>
public sealed class ConfirmReleasePopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject questionText;
	[SerializeField] private GameObject line;
	[SerializeField] private GameObject purposeNameText;
	[SerializeField] private GameObject prpsIcon;
	[SerializeField] private GameObject currentPRPSText;
	[SerializeField] private GameObject additionPRPSText;
	[SerializeField] private GameObject confirmButton;
	[SerializeField] private GameObject cancelButton;
	[SerializeField] private GameObject confirmText;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		questionText.AnimateScaleX(1f, 0.15f);
		line.AnimateScaleX(1f, 0.175f);
		purposeNameText.AnimateGraphicAndScale(1f, 1f, 0.2f);
		prpsIcon.AnimateGraphicAndScale(1f, 1f, 0.225f);
		currentPRPSText.AnimateGraphicAndScale(1f, 1f, 0.25f);
		additionPRPSText.AnimateGraphicAndScale(1f, 1f, 0.275f);

		if (confirmText.activeInHierarchy)
		{
			confirmText.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
		}
		else
		{
			confirmButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
			cancelButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
		}
	}
}
