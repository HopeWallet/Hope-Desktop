using UnityEngine;

public class HopeWalletForm : FormAnimation
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject importWalletButton;
	[SerializeField] private GameObject importWalletDesc;
	[SerializeField] private GameObject createWalletButton;
	[SerializeField] private GameObject createWalletDesc;

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f));

		importWalletButton.AnimateScaleX(1f, 0.2f,
			() => importWalletDesc.AnimateScaleX(1f, 0.2f,
			() => createWalletButton.AnimateScaleX(1f, 0.2f,
			() => createWalletDesc.AnimateScaleX(1f, 0.2f, FinishedAnimatingIn))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		createWalletDesc.AnimateScaleX(0f, 0.1f,
			() => createWalletButton.AnimateScaleX(0f, 0.1f));

		importWalletDesc.AnimateScaleX(0f, 0.1f,
			() => importWalletButton.AnimateScaleX(0f, 0.1f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f, FinishedAnimatingOut))));
	}

}
