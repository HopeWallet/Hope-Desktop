using UnityEngine;

/// <summary>
/// The animator class of the ConfirmLockPopup
/// </summary>
public sealed class ConfirmLockPopupAnimator : CountdownTimerAnimator
{
	[SerializeField] private GameObject prpsSection;
	[SerializeField] private GameObject dubiSection;
	[SerializeField] private GameObject noteText;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		AnimateSection(prpsSection);
		AnimateSection(dubiSection);
		noteText.AnimateScaleX(1f, 0.25f);

		CoroutineUtils.ExecuteAfterWait(0.2f, StartTimerAnimation);

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

	/// <summary>
	/// Animates the child objects of the given section
	/// </summary>
	/// <param name="tokenSection"> The given section being animated in </param>
	private void AnimateSection(GameObject tokenSection)
	{
		float duration = 0.1f;
		for (int i = 0; i < tokenSection.transform.childCount; i++)
		{
			tokenSection.transform.GetChild(i).gameObject.AnimateGraphicAndScale(1f, 1f, duration);
			duration += 0.04f;
		}
	}
}
