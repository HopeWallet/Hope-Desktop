using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the ReEnterPasswordMenu
/// </summary>
public sealed class ReEnterPasswordMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject plainBackground;
	[SerializeField] private HopeInputField passwordInputField;
	[SerializeField] private GameObject unlockButton;
	[SerializeField] private GameObject homeButton;
	[SerializeField] private GameObject loadingIcon;

    private void Start()
    {
        GetComponent<ReEnterPasswordMenu>().OnPasswordVerificationStarted += VerifyingPassword;
        GetComponent<ReEnterPasswordMenu>().OnPasswordEnteredIncorrect += PasswordIncorrect;
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
		base.AnimateOut();

		FinishedAnimating();
	}

	/// <summary>
	/// Called if the password is incorrect
	/// </summary>
	private void PasswordIncorrect()
	{
		passwordInputField.Error = true;
		passwordInputField.UpdateVisuals();
		unlockButton.GetComponent<Button>().interactable = false;
		VerifyingPassword();
	}

	/// <summary>
	/// Animates the loadingIcon while in or out of view
	/// </summary>
	private void VerifyingPassword()
	{
		bool startingProcess = !loadingIcon.activeInHierarchy;

		if (startingProcess)
		{
			loadingIcon.SetActive(true);
			Animating = true;
			passwordInputField.InputFieldBase.DeactivateInputField();
		}

		loadingIcon.AnimateGraphicAndScale(startingProcess ? 1f : 0f, startingProcess ? 1f : 0f, 0.15f);
		unlockButton.AnimateGraphicAndScale(startingProcess ? 0f : 1f, startingProcess ? 0f : 1f, 0.15f);

		if (!startingProcess)
		{
			loadingIcon.SetActive(false);
			Animating = false;
		}
	}
}