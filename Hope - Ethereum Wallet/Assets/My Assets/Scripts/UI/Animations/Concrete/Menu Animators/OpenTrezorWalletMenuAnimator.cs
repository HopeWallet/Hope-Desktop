using UnityEngine;

/// <summary>
/// The animator class of the OpenTrezorWalletMenu
/// </summary>
public sealed class OpenTrezorWalletMenuAnimator : OpenHardwareWalletMenuBaseAnimator<OpenTrezorWalletMenu, TrezorWallet>
{
	[SerializeField] private GameObject backButton;
	[SerializeField] private GameObject trezorLogo;
	[SerializeField] private GameObject enterPINText;
	[SerializeField] private GameObject subText;
	[SerializeField] private Transform keyPadButtons;
	[SerializeField] private GameObject plugInTrezorText;
	[SerializeField] private GameObject passcodeInputField;
	[SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject loadingPinIcon;

    private bool pinSectionEnabled;

	private void Start() => GetComponent<OpenTrezorWalletMenu>().TrezorPINSectionOpening += AnimateEnterPINSection;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		backButton.AnimateGraphicAndScale(1f, 1f, 0.2f);
		trezorLogo.AnimateGraphicAndScale(1f, 1f, 0.2f);
		plugInTrezorText.AnimateGraphicAndScale(1f, 1f, 0.25f);
		awaitingConnectionText.AnimateGraphicAndScale(1f, 1f, 0.3f);
		loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.35f, FinishedAnimating);
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		backButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
		trezorLogo.AnimateGraphicAndScale(0f, 0f, 0.3f);

		enterPINText.AnimateGraphicAndScale(0f, 0f, 0.3f);
		subText.AnimateGraphicAndScale(0f, 0f, 0.3f);

		for (int i = 0; i < 9; i++)
			keyPadButtons.GetChild(i).gameObject.AnimateGraphicAndScale(0f, 0f, 0.3f);

		passcodeInputField.AnimateScaleX(0f, 0.3f);
		nextButton.AnimateGraphicAndScale(0f, 0f, 0.3f);

		plugInTrezorText.AnimateGraphicAndScale(0f, 0f, 0.3f);
		awaitingConnectionText.AnimateGraphicAndScale(0f, 0f, 0.3f);
		loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.3f);
		deviceConnectedText.AnimateGraphicAndScale(0f, 0f, 0.3f);
		openWalletButton.AnimateGraphicAndScale(0f, 0f, 0.3f, FinishedAnimating);
	}

    protected override void ChangeLoadStatus(bool loadingWallet)
    {
        Animating = loadingWallet;

        if (pinSectionEnabled)
        {
            SwitchObjects(loadingWallet ? nextButton : loadingPinIcon, loadingWallet ? loadingPinIcon : nextButton);
        }
        else
        {
            SwitchObjects(loadingWallet ? deviceConnectedText : loadingWalletText, loadingWallet ? loadingWalletText : awaitingConnectionText);
            SwitchObjects(loadingWallet ? openWalletButton : loadingIcon, loadingWallet ? loadingIcon : loadingIcon);
        }
    }

    /// <summary>
    /// Animates the enter PIN section for the Trezor
    /// </summary>
    private void AnimateEnterPINSection()
	{
        Animating = true;
        pinSectionEnabled = true;

        enterPINText.AnimateGraphicAndScale(1f, 1f, 0.25f);
		subText.AnimateGraphicAndScale(1f, 1f, 0.275f);
        plugInTrezorText.AnimateGraphicAndScale(0f, 0f, 0.3f);
        awaitingConnectionText.AnimateGraphicAndScale(0f, 0f, 0.3f);
        loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.3f);
        deviceConnectedText.AnimateGraphicAndScale(0f, 0f, 0.3f);
        openWalletButton.AnimateGraphicAndScale(0f, 0f, 0.3f);
        loadingWalletText.AnimateGraphicAndScale(0f, 0f, 0.3f);

        float duration = 0.3f;
		for (int i = 0; i < 9; i++)
		{
			keyPadButtons.GetChild(i).gameObject.AnimateGraphicAndScale(1f, 1f, duration);

			if (i == 2 || i == 5)
				duration += 0.025f;
		}

		passcodeInputField.AnimateScaleX(1f, 0.375f);
        nextButton.AnimateGraphicAndScale(1f, 1f, 0.4f, () => Animating = false);
	}
}