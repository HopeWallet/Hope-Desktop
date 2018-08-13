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

	protected override void AnimateUniqueElementsIn()
	{
		throw new System.NotImplementedException();
	}

	protected override void AnimateUniqueElementsOut()
	{
		throw new System.NotImplementedException();
	}

	/// <summary>
	/// Animates the error icon
	/// </summary>
	/// <param name="icon"> The icon being animated </param>
	/// <param name="isValidField"> Whether animating the icon in or out </param>
	private void AnimateErrorIcon(InteractableIcon icon, bool isValidField) => icon.AnimateIcon(isValidField ? 0f : 1f);
}
