using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the ReEnterPasswordMenu
/// </summary>
public sealed class ReEnterPasswordMenuAnimator : MenuAnimator
{
	[SerializeField] private GameObject plainBackground;
	[SerializeField] private GameObject messageText;
	[SerializeField] private HopeInputField passwordInputField;
	[SerializeField] private GameObject unlockButton;
	[SerializeField] private GameObject homeButton;
	[SerializeField] private GameObject loadingIcon;

	/// <summary>
	/// Sets the necessary events
	/// </summary>
	private void Start()
	{
		GetComponent<ReEnterPasswordMenu>().OnPasswordVerificationStarted += VerifyingPassword;
		GetComponent<ReEnterPasswordMenu>().OnPasswordEnteredIncorrect += PasswordIncorrect;
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		plainBackground.AnimateGraphic(1f, 0.2f);
		messageText.AnimateGraphicAndScale(1f, 1f, 0.25f);
		passwordInputField.InputFieldBase.ActivateInputField();
		passwordInputField.gameObject.AnimateScale(1f, 0.3f);
		homeButton.AnimateGraphicAndScale(1f, 1f, 0.35f);
		unlockButton.AnimateGraphicAndScale(1f, 1f, 0.35f, FinishedAnimating);
	}

	/// <summary>
	/// Animate the unique elements of the form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		if (loadingIcon.activeInHierarchy)
			loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.2f);

		plainBackground.AnimateGraphic(0f, 0.2f);
		messageText.AnimateGraphicAndScale(0f, 0f, 0.2f);
		passwordInputField.gameObject.AnimateScale(0f, 0.2f);
		homeButton.AnimateGraphicAndScale(0f, 0f, 0.2f);
		unlockButton.AnimateGraphicAndScale(0f, 0f, 0.2f, FinishedAnimating);
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
			unlockButton.AnimateGraphicAndScale(0f, 0f, 0.15f, () => loadingIcon.AnimateGraphicAndScale(1f, 1f, 0.15f));
			passwordInputField.InputFieldBase.DeactivateInputField();
		}

		if (!startingProcess)
		{
			loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.15f, () =>
			{
				loadingIcon.SetActive(false);
				unlockButton.AnimateGraphicAndScale(1f, 1f, 0.15f);
				Animating = false;
			});
		}
	}
}