using UnityEngine;
using UnityEngine.UI;

public class ReEnterPasswordMenuAnimator : UIAnimator
{
	[SerializeField] private GameObject walletName;
	[SerializeField] private GameObject line;
	[SerializeField] private HopeInputField passwordInputField;
	[SerializeField] private GameObject unlockButton;
	[SerializeField] private GameObject homeButton;
	[SerializeField] private GameObject loadingIcon;

	/// <summary>
	/// Animates the unique elements of this form into view
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

	/// <summary>
	/// Called if the password is incorrect
	/// </summary>
	public void PasswordIncorrect()
	{
		passwordInputField.Error = true;
		passwordInputField.UpdateVisuals();
		unlockButton.GetComponent<Button>().interactable = false;
		VerifyingPassword();
	}

	/// <summary>
	/// Animates the loadingIcon while in or out of view
	/// </summary>
	public void VerifyingPassword()
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