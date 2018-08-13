using UnityEngine;
using UnityEngine.UI;

public class LockPRPSPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject prpsTokenSection;
	[SerializeField] private GameObject dubiTokenSection;
	[SerializeField] private GameObject purposeSection;
	[SerializeField] private GameObject transactionSpeedSection;
	[SerializeField] private GameObject timePeriodSection;
	[SerializeField] private GameObject noteText;
	[SerializeField] private GameObject lockPRPSButton;

	[SerializeField] private InteractableIcon purposeErrorIcon;

    private void Start()
    {
        LockPRPSPopup lockPRPSPopup = GetComponent<LockPRPSPopup>();
        lockPRPSPopup.Amount.OnLockAmountChanged += () => AnimateErrorIcon(purposeErrorIcon, lockPRPSPopup.Amount.IsValid || lockPRPSPopup.Amount.IsEmpty);
    }

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		prpsTokenSection.AnimateScale(1f, 0.1f);
		dubiTokenSection.AnimateScale(1f, 0.1f);
		purposeSection.AnimateScaleX(1f, 0.15f);
		transactionSpeedSection.AnimateScaleX(1f, 0.2f);
		timePeriodSection.AnimateScaleX(1f, 0.25f);
		noteText.AnimateScaleX(1f, 0.275f);
		lockPRPSButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the unique elements of this form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		lockPRPSButton.AnimateGraphicAndScale(0f, 0f, 0.1f);
		noteText.AnimateScaleX(0f, 0.125f);
		timePeriodSection.AnimateScaleX(0f, 0.15f);
		transactionSpeedSection.AnimateScaleX(0f, 0.2f, () => AnimateBasicElements(false));
		purposeSection.AnimateScaleX(0f, 0.25f);
		dubiTokenSection.AnimateScale(0f, 0.3f);
		prpsTokenSection.AnimateScale(0f, 0.3f);
	}

	/// <summary>
	/// Animates the error icon
	/// </summary>
	/// <param name="icon"> The icon being animated </param>
	/// <param name="isValidField"> Whether animating the icon in or out </param>
	private void AnimateErrorIcon(InteractableIcon icon, bool isValidField) => icon.AnimateIcon(isValidField ? 0f : 1f);
}
