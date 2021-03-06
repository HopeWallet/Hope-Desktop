﻿using UnityEngine;

/// <summary>
/// The animator class of the OpenLedgerWalletMenu
/// </summary>
public sealed class OpenLedgerWalletMenuAnimator : OpenHardwareWalletMenuBaseAnimator<OpenLedgerWalletMenu, LedgerWallet>
{
	[SerializeField] private GameObject backButton;
	[SerializeField] private GameObject ledgerLogo;
	[SerializeField] private GameObject step1Text;
	[SerializeField] private GameObject step2Text;
	[SerializeField] private GameObject step3Text;
	[SerializeField] private GameObject step4Text;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		backButton.AnimateGraphicAndScale(1f, 1f, 0.2f);
		ledgerLogo.AnimateGraphicAndScale(1f, 1f, 0.2f);

		step1Text.AnimateGraphicAndScale(1f, 1f, 0.275f);
		step2Text.AnimateGraphicAndScale(1f, 1f, 0.3f);
		step3Text.AnimateGraphicAndScale(1f, 1f, 0.325f);
		step4Text.AnimateGraphicAndScale(1f, 1f, 0.35f);
		awaitingConnectionText.AnimateGraphicAndScale(1f, 1f, 0.375f);
		loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.4f, FinishedAnimating);
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		backButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		ledgerLogo.AnimateGraphicAndScale(0f, 0f, 0.3f);

		step1Text.AnimateGraphicAndScale(0f, 0f, 0.3f);
		step2Text.AnimateGraphicAndScale(0f, 0f, 0.3f);
		step3Text.AnimateGraphicAndScale(0f, 0f, 0.3f);
		step4Text.AnimateGraphicAndScale(0f, 0f, 0.3f);
		awaitingConnectionText.AnimateGraphicAndScale(0f, 0f, 0.3f);
		loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.3f);
		deviceConnectedText.AnimateGraphicAndScale(0f, 0f, 0.3f);
		openWalletButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
	}
}