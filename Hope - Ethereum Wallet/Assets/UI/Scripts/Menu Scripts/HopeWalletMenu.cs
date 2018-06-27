using UnityEngine;

public class HopeWalletMenu : MenuAnimation
{
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject importWalletButton;
	[SerializeField] private GameObject importWalletDesc;
	[SerializeField] private GameObject createWalletButton;
	[SerializeField] private GameObject createWalletDesc;

	protected override void AnimateIn()
	{
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f));

		importWalletButton.AnimateScaleX(1f, 0.2f,
			() => importWalletDesc.AnimateScaleX(1f, 0.2f,
			() => createWalletButton.AnimateScaleX(1f, 0.2f,
			() => createWalletDesc.AnimateScaleX(1f, 0.2f,
			() => FinishedAnimatingIn()))));
	}

	protected override void AnimateOut()
	{
		createWalletDesc.AnimateScaleX(0f, 0.1f,
			() => createWalletButton.AnimateScaleX(0f, 0.1f));

		importWalletDesc.AnimateScaleX(0f, 0.1f,
			() => importWalletButton.AnimateScaleX(0f, 0.1f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => FinishedAnimatingOut()))));
	}

	/// <summary>
	/// Disables the menu and proceeds to next menu
	/// </summary>
	/// <param name="importing">Checks if user is importing wallet or creating new wallet</param>
	public void ImportOrCreateClicked(bool importing)
	{
		DisableMenu();

		//if (import)
			//Open up importing wallet menu

		//else
			//Open up creating wallet menu
	}


	/// <summary>
	/// Disables menu and moves to previous menu
	/// </summary>
	public void BackButton()
	{
		DisableMenu();

		//Go back to saved wallets menu, or choose wallet menu, depending on if first time or not
	}
}
