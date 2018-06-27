using UnityEngine;

public class ChooseWalletMenu : MenuAnimation
{

	[SerializeField] private GameObject vignette;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject ledgerButton;
	[SerializeField] private GameObject hopeButton;

	#region Animating

	protected override void AnimateIn()
	{
		vignette.AnimateGraphic(1f, 0.5f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
			() => ledgerButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => hopeButton.AnimateGraphicAndScale(1f, 1f, 0.2f, 
			() => FinishedAnimatingIn()))));
	}

	protected override void AnimateOut()
	{
		hopeButton.AnimateGraphicAndScale(0f, 0.1f, 0.2f);
		ledgerButton.AnimateGraphicAndScale(0f, 0.1f, 0.2f);
		title.AnimateGraphicAndScale(0f, 0.1f, 0.2f,
			() => form.AnimateGraphicAndScale(0f, 0.1f, 0.2f,
			() => FinishedAnimatingOut()));
	}

	#endregion

	/// <summary>
	/// Disables the menu and opens up the hope wallets menu, or ledger menu
	/// </summary>
	/// <param name="hopeWalletClicked">Checks to see if the hope wallet button has been clicked</param>
	public void ButtonClicked(bool hopeWalletClicked)
	{
		DisableMenu();

		//if (hopeWalletClicked)
			//Add Wallets menu next

		//else 
			//Show ledger menu next
	}

}
