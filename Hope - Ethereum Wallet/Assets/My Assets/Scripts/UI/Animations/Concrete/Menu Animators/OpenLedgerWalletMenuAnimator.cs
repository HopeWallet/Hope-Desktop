
using TMPro;
using UnityEngine;

public class OpenLedgerWalletMenuAnimator : UIAnimator
{
	[SerializeField] private GameObject awaitingConnectionText;
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
		awaitingConnectionText.GetComponent<LoadingTextAnimator>().enabled = !ledgerConnected;

		awaitingConnectionText.AnimateGraphicAndScale(0f, 0f, 0.15f, () =>
		{
			awaitingConnectionText.GetComponent<TextMeshProUGUI>().text = ledgerConnected ? "Device found" : "Awaiting connection";
			awaitingConnectionText.AnimateGraphicAndScale(1f, 1f, 0.15f);
		});

		if (ledgerConnected)
			loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.15f, () => openWalletButton.AnimateGraphicAndScale(1f, 1f, 0.15f));
		else
			openWalletButton.AnimateGraphicAndScale(0f, 0f, 0.15f, () => loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.15f));
	}


}