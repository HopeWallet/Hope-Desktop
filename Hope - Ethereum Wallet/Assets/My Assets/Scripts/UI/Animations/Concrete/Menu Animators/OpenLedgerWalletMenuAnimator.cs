﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the OpenLedgerWalletMenu
/// </summary>
public sealed class OpenLedgerWalletMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject backButton;
	[SerializeField] private GameObject ledgerLogo;
	[SerializeField] private GameObject step1Text;
	[SerializeField] private GameObject step2Text;
	[SerializeField] private GameObject step3Text;
	[SerializeField] private GameObject step4Text;
	[SerializeField] private GameObject awaitingConnectionText;
	[SerializeField] private GameObject deviceConnectedText;
	[SerializeField] private GameObject loadingIcon;
	[SerializeField] private GameObject openWalletButton;

	private void Awake()
	{
		var openLedgerWalletMenu = transform.GetComponent<OpenLedgerWalletMenu>();

		openLedgerWalletMenu.OnLedgerConnected += () => ChangeLedgerStatus(true);
		openLedgerWalletMenu.OnLedgerDisconnected += () => ChangeLedgerStatus(false);
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateIn()
	{
		backButton.AnimateGraphicAndScale(1f, 1f, 0.2f);
		ledgerLogo.AnimateGraphicAndScale(1f, 1f, 0.2f);

		base.AnimateIn();

		step1Text.AnimateGraphicAndScale(1f, 1f, 0.275f);
		step2Text.AnimateGraphicAndScale(1f, 1f, 0.3f);
		step3Text.AnimateGraphicAndScale(1f, 1f, 0.325f);
		step4Text.AnimateGraphicAndScale(1f, 1f, 0.35f);
		awaitingConnectionText.AnimateGraphicAndScale(1f, 1f, 0.375f);
		loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.4f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		backButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		ledgerLogo.AnimateGraphicAndScale(0f, 0f, 0.3f);

		base.AnimateOut();

		step1Text.AnimateGraphicAndScale(0f, 0f, 0.3f);
		step2Text.AnimateGraphicAndScale(0f, 0f, 0.3f);
		step3Text.AnimateGraphicAndScale(0f, 0f, 0.3f);
		step4Text.AnimateGraphicAndScale(0f, 0f, 0.3f);
		awaitingConnectionText.AnimateGraphicAndScale(0f, 0f, 0.3f);
		loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.3f);
		deviceConnectedText.AnimateGraphicAndScale(0f, 0f, 0.3f);
		openWalletButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
	}

	private void ChangeLedgerStatus(bool ledgerConnected)
	{
		SwitchObjects(ledgerConnected ? awaitingConnectionText : deviceConnectedText, ledgerConnected ? deviceConnectedText : awaitingConnectionText);
		SwitchObjects(ledgerConnected ? loadingIcon : openWalletButton, ledgerConnected ? openWalletButton : loadingIcon);
	}

	private void SwitchObjects(GameObject gameObjectOut, GameObject gameObjectIn)
	{
		gameObjectOut.AnimateGraphicAndScale(0f, 0f, 0.15f, () => gameObjectIn.AnimateGraphicAndScale(1f, 1f, 0.15f));
	}
}