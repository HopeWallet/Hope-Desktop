using UnityEngine;
using UnityEngine.UI;

public class ReceiveAssetPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject checkmarkIcon;
	[SerializeField] private GameObject qrCodeSection;
	[SerializeField] private GameObject copyAddressButton;

	/// <summary>
	/// Initializes the copyAddress button listener
	/// </summary>
	private void Awake() => copyAddressButton.GetComponent<Button>().onClick.AddListener(AnimateCheckMarkIcon);

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		blur.AnimateMaterialBlur(1.25f, 0.15f);
		dim.AnimateGraphic(1f, 0.15f);
		form.AnimateGraphicAndScale(1f, 1f, 0.15f,
			() => title.AnimateGraphicAndScale(1f, 0.85f, 0.15f,
			() => addressSection.AnimateScaleX(1f, 0.15f,
			() => qrCodeSection.AnimateGraphicAndScale(1f, 1f, 0.15f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		checkmarkIcon.AnimateGraphic(0f, 0.3f);

		qrCodeSection.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => addressSection.AnimateScaleX(0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => { blur.AnimateMaterialBlur(0f, 0.15f); dim.AnimateGraphic(0f, 0.15f, FinishedAnimating); }))));
	}

	/// <summary>
	/// Animates the check mark icon on and off screen
	/// </summary>
	private void AnimateCheckMarkIcon()
	{
		copyAddressButton.GetComponent<Button>().interactable = false;

		checkmarkIcon.transform.localScale = new Vector3(0, 0, 1);

		checkmarkIcon.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => checkmarkIcon.AnimateScaleX(1.01f, 0.5f,
			() => checkmarkIcon.AnimateGraphic(0f, 0.5f,
			() => copyAddressButton.GetComponent<Button>().interactable = true)));
	}
}
