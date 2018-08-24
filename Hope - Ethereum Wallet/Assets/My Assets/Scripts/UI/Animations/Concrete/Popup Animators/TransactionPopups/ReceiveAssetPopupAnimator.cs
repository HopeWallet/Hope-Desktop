using UnityEngine;
using UnityEngine.UI;

public class ReceiveAssetPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject noteText;
	[SerializeField] private GameObject line1;
	[SerializeField] private GameObject addressInputField;
	[SerializeField] private GameObject copyButton;
	[SerializeField] private GameObject checkMarkIcon;
	[SerializeField] private GameObject line2;
	[SerializeField] private GameObject qrCodeText;
	[SerializeField] private GameObject qrCodeImage;

	private bool animatingIcon;

	/// <summary>
	/// Initializes the copyAddress button listener
	/// </summary>
	private void Awake() => copyButton.GetComponent<Button>().onClick.AddListener(() => { if (!animatingIcon) AnimateCheckMarkIcon(); });

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		noteText.AnimateGraphicAndScale(1f, 1f, 0.1f);
		line1.AnimateScaleX(1f, 0.125f);
		addressInputField.AnimateScaleX(1f, 0.15f);
		copyButton.AnimateGraphicAndScale(1f, 1f, 0.2f);
		line2.AnimateScaleX(1f, 0.225f);
		qrCodeText.AnimateScaleX(1f, 0.25f);
		qrCodeImage.AnimateScale(1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Animates the check mark icon on and off screen
	/// </summary>
	private void AnimateCheckMarkIcon()
	{
		animatingIcon = true;
		checkMarkIcon.transform.localScale = new Vector3(0, 0, 1);

		checkMarkIcon.AnimateGraphicAndScale(1f, 1f, 0.15f);
		CoroutineUtils.ExecuteAfterWait(0.6f, () => { if (checkMarkIcon != null) checkMarkIcon.AnimateGraphic(0f, 0.25f, () => animatingIcon = false); });
	}
}
