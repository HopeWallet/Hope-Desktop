using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which animates the CreateWalletMenu.
/// </summary>
public class CreateWalletMenuAnimator : UIAnimator
{
	[SerializeField] private GameObject walletNameField;
	[SerializeField] private GameObject passwordHeader;
	[SerializeField] private GameObject password1Field;
	[SerializeField] private GameObject password2Field;
	[SerializeField] private GameObject createButton;

	[SerializeField] private GameObject passwordStrengthSection;
	[SerializeField] private GameObject progressBar;
	[SerializeField] private GameObject progressBarFull;
	[SerializeField] private TextMeshProUGUI passwordStrengthText;

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		walletNameField.AnimateScaleX(1f, 0.1f);
		passwordHeader.AnimateScaleX(1f, 0.15f);
		passwordStrengthSection.AnimateScaleX(1f, 0.15f);
		password1Field.AnimateScaleX(1f, 0.2f);
		password2Field.AnimateScaleX(1f, 0.2f);
		createButton.AnimateGraphicAndScale(1f, 1f, 0.25f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the unique elements of this form out of view
	/// </summary>
	protected override void AnimateUniqueElementsOut()
	{
		FinishedAnimating();

		walletNameField.SetScale(new Vector2(0f, 1f));
		passwordHeader.SetScale(new Vector2(0f, 1f));
		passwordStrengthSection.SetScale(new Vector2(0f, 1f));
		password1Field.SetScale(new Vector2(0f, 1f));
		password2Field.SetScale(new Vector2(0f, 1f));
		createButton.SetGraphicAndScale(Vector2.zero);
	}

	/// <summary>
	/// Animates the password strength bar depending on what the input is in the first password field
	/// </summary>
	/// <param name="password"> The text in the first password field </param>
	public void AnimatePasswordStrengthBar(string password)
	{
		progressBar.AnimateTransformX(string.IsNullOrEmpty(password) ? 0f : -35f, 0.15f);
		passwordStrengthText.gameObject.AnimateGraphic(string.IsNullOrEmpty(password) ? 0f : 1f, 0.15f);

		float lengthPercentage = 0.05f * password.Length;
		float colorPercentage = lengthPercentage >= 0.5f ? (lengthPercentage - 0.5f) * 2f : lengthPercentage * 2f;

		Color strengthColor = Color.Lerp(lengthPercentage >= 0.5f ? UIColors.Yellow : UIColors.Red, lengthPercentage >= 0.5f ? UIColors.Green : UIColors.Yellow, colorPercentage);

		if (lengthPercentage <= 1f)
		{
			progressBarFull.AnimateScaleX(lengthPercentage, 0.1f);
			progressBarFull.GetComponent<Image>().color = strengthColor;
		}

		passwordStrengthText.text = lengthPercentage == 0f ? "" : lengthPercentage < 0.4f ? "Too Short" : lengthPercentage < 0.55f ? "Weak" : lengthPercentage < 0.7f ? "Fair" : lengthPercentage < 0.85f ? "Strong" : "Very Strong";
	}

	/// <summary>
	/// Animates an icon in or out of view
	/// </summary>
	/// <param name="icon"> The icon being animated </param>
	/// <param name="animateIn"> Whether the icon is being animated in or out </param>
	public void AnimateIcon(InteractableIcon icon, bool animateIn) => icon.AnimateIcon(animateIn ? 1f : 0f);
}
