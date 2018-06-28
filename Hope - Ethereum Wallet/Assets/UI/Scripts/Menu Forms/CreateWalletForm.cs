using UnityEngine;

public class CreateWalletForm : FormAnimation
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject walletNameField;
	[SerializeField] private GameObject passwordHeader;
	[SerializeField] private GameObject password1Field;
	[SerializeField] private GameObject password2Field;
	[SerializeField] private GameObject createButton;
	[SerializeField] private GameObject warningSign;

	#region Animating

	protected override void AnimateIn()
	{
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
			() => createButton.AnimateGraphicAndScale(1f, 1f, 0.2f)));

		walletNameField.AnimateScaleX(1f, 0.15f,
			() => passwordHeader.AnimateScaleX(1f, 0.15f,
			() => password1Field.AnimateScaleX(1f, 0.15f,
			() => password2Field.AnimateScaleX(1f, 0.15f,
			() => FinishedAnimatingIn()))));

	}

	protected override void AnimateOut()
	{
		createButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => FinishedAnimatingOut())));

		walletNameField.AnimateScaleX(0f, 0.15f);
		passwordHeader.AnimateScaleX(0f, 0.15f);
		password1Field.AnimateScaleX(0f, 0.15f);
		password2Field.AnimateScaleX(0f, 0.15f);
	}

	#endregion

}
