using UnityEngine;

/// <summary>
/// The animator class of the OpenLedgerWalletMenu
/// </summary>
public sealed class OpenLedgerWalletMenuAnimator : MenuAnimator
{
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
		base.AnimateIn();

		FinishedAnimating();
	}

	/// <summary>
	/// Animates the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		FinishedAnimating();
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