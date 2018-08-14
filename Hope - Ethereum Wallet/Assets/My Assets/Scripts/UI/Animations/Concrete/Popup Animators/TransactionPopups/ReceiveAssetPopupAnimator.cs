using UnityEngine;
using UnityEngine.UI;

public class ReceiveAssetPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject addressInputField;
	[SerializeField] private GameObject copyAddressButton;
	[SerializeField] private GameObject checkMarkIcon;
	[SerializeField] private GameObject qrCodeText;
	[SerializeField] private GameObject qrCodeImage;

	/// <summary>
	/// Initializes the copyAddress button listener
	/// </summary>
	private void Awake() => copyAddressButton.GetComponent<Button>().onClick.AddListener(AnimateCheckMarkIcon);

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		addressInputField.AnimateScaleX(1f, 0.15f);
		copyAddressButton.AnimateGraphicAndScale(1f, 1f, 0.2f);
		qrCodeText.AnimateScaleX(1f, 0.25f);
		qrCodeImage.AnimateScale(1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the check mark icon on and off screen
	/// </summary>
	private void AnimateCheckMarkIcon()
	{
		copyAddressButton.GetComponent<Button>().interactable = false;

		checkMarkIcon.transform.localScale = new Vector3(0, 0, 1);

		checkMarkIcon.AnimateGraphicAndScale(1f, 1f, 0.2f);
		CoroutineUtils.ExecuteAfterWait(0.7f, () => checkMarkIcon.AnimateGraphic(0f, 0.5f,
			() => copyAddressButton.GetComponent<Button>().interactable = true));
	}
}
