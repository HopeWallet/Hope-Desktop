using UnityEngine;
using UnityEngine.UI;

public class ReceiveAssetPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject checkmarkIcon;
	[SerializeField] private GameObject qrCodeSection;
	[SerializeField] private GameObject copyAddressButton;

	/// <summary>
	/// Initializes the copyAddress button listener
	/// </summary>
	private void Awake() => copyAddressButton.GetComponent<Button>().onClick.AddListener(AnimateCheckMarkIcon);

	protected override void AnimateUniqueElementsIn()
	{
		throw new System.NotImplementedException();
	}

	protected override void AnimateUniqueElementsOut()
	{
		throw new System.NotImplementedException();
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
