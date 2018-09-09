
using TMPro;
using UnityEngine;

public class OpenLedgerWalletMenuAnimator : UIAnimator
{
	[SerializeField] private GameObject awaitingConnectionText;
	[SerializeField] private GameObject deviceConnectedText;
	[SerializeField] private GameObject loadingIcon;
	[SerializeField] private GameObject openWalletButton;

	private OpenLedgerWalletMenu openLedgerWalletMenu;

	private void Awake()
	{
		openLedgerWalletMenu.transform.GetComponent<OpenLedgerWalletMenu>();

		openLedgerWalletMenu.OnLedgerConnected += () => ChangeLedgerStatus(true);
		openLedgerWalletMenu.OnLedgerDisconnected += () => ChangeLedgerStatus(false);
	}

	/// <summary>
	/// Animates the unique elements into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		FinishedAnimating();
	}

	/// <summary>
	/// Resets the unique elements of the form back to the starting positions
	/// </summary>
	protected override void ResetElementValues()
	{
		FinishedAnimating();
	}

	private void ChangeLedgerStatus(bool ledgerConnected)
	{
		SwitchObjects(ledgerConnected ? loadingIcon : openWalletButton, ledgerConnected ? openWalletButton : loadingIcon);
		SwitchObjects(ledgerConnected ? awaitingConnectionText : deviceConnectedText, ledgerConnected ? deviceConnectedText : awaitingConnectionText);
	}

	private void SwitchObjects(GameObject gameObjectOut, GameObject gameObjectIn)
	{
		gameObjectOut.AnimateGraphicAndScale(0f, 0f, 0.15f, () => gameObjectIn.AnimateGraphicAndScale(1f, 1f, 0.15f));
	}
}