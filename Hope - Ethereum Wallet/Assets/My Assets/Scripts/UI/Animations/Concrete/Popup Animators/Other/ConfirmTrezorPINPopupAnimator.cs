using UnityEngine;

/// <summary>
/// The animator class of the ConfirmTrezorPINPopup
/// </summary>
public class ConfirmTrezorPINPopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject enterPINText;
	[SerializeField] private GameObject subText;
	[SerializeField] private Transform keyPadButtons;
	[SerializeField] private GameObject passcodeInputField;
	[SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject loadingPinIcon;

    private void Start()
    {
        var trezorMenu = GetComponent<EnterTrezorPINPopup>();

        trezorMenu.CheckingPIN += () => UpdatePINSection(true);
        trezorMenu.ReloadPINSection += () => UpdatePINSection(false);
    }

    /// <summary>
    /// Animates the unique elements of this form into view
    /// </summary>
    protected override void AnimateUniqueElementsIn()
	{
		enterPINText.AnimateGraphicAndScale(1f, 1f, 0.25f);
		subText.AnimateGraphicAndScale(1f, 1f, 0.275f);

		float duration = 0.3f;
		for (int i = 0; i < 9; i++)
		{
			keyPadButtons.GetChild(i).gameObject.AnimateGraphicAndScale(1f, 1f, duration);

			if (i == 2 || i == 5)
				duration += 0.025f;
		}

		passcodeInputField.AnimateScaleX(1f, 0.375f);
		confirmButton.AnimateGraphicAndScale(1f, 1f, 0.4f, FinishedAnimating);
	}

    private void UpdatePINSection(bool loadingWallet)
    {
        Animating = loadingWallet;

        SwitchObjects(loadingWallet ? confirmButton : loadingPinIcon, loadingWallet ? loadingPinIcon : confirmButton);
    }

    /// <summary>
    /// Switches one object with another
    /// </summary>
    /// <param name="gameObjectOut"> The object being animated out </param>
    /// <param name="gameObjectIn"> The object being animated in </param>
    protected void SwitchObjects(GameObject gameObjectOut, GameObject gameObjectIn)
    {
        gameObjectOut.AnimateGraphicAndScale(0f, 0f, 0.15f, () => gameObjectIn.AnimateGraphicAndScale(1f, 1f, 0.15f));
    }
}
