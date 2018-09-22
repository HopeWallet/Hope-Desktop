﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the ConfirmLockPopup
/// </summary>
public sealed class ConfirmLockPopupAnimator : CountdownTimerAnimator
{
	[SerializeField] private GameObject prpsSection;
	[SerializeField] private GameObject dubiSection;
	[SerializeField] private GameObject noteText;

	/// <summary>
	/// Sets the confirm button to interactable if countdown timer setting has been disabled
	/// </summary>
	private void Awake()
	{
		if (!SecurePlayerPrefs.GetBool("countdown timer"))
			confirmButton.GetComponent<Button>().interactable = true;
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		prpsSection.AnimateScale(1f, 0.2f);
		dubiSection.AnimateScale(1f, 0.2f, StartTimerAnimation);
		noteText.AnimateScaleX(1f, 0.25f);
		confirmButton.AnimateGraphicAndScale(1f, 1f, 0.3f);
		cancelButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}
}
