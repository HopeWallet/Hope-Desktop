using TMPro;
using UnityEngine;

public class SendTokenPopupAnimator : UIAnimator
{

	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;

	[SerializeField] private GameObject tokenSection;
	[SerializeField] private GameObject advancedModeSection;
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject amountSection;
	[SerializeField] private GameObject gasLimitSection;
	[SerializeField] private GameObject gasPriceSection;
	[SerializeField] private GameObject transactionSpeedSection;

	[SerializeField] private GameObject sendButton;

	[SerializeField] private GameObject advancedModeToggle;
	[SerializeField] private GameObject maxToggle;

	private bool advancedMode;

	private void Awake()
	{
		advancedModeToggle.transform.GetComponent<ToggleAnimator>().ToggleClick = AdvancedModeClicked;
		maxToggle.transform.GetComponent<ToggleAnimator>().ToggleClick = MaxClicked;
	}

	protected override void AnimateIn()
	{
		FinishedAnimating();
	}

	protected override void AnimateOut()
	{
	}

	private void AdvancedModeClicked()
	{
		advancedMode = !advancedMode;
		Animating = true;

		if (advancedMode)
		{
			transactionSpeedSection.AnimateGraphicAndScale(0f, 0f, 0.1f,
				() => gasLimitSection.AnimateGraphicAndScale(1f, 1f, 0.1f,
				() => gasPriceSection.AnimateGraphicAndScale(1f, 1f, 0.1f,
				() => Animating = false)));
		}

		else
		{
			gasLimitSection.AnimateGraphicAndScale(0f, 0f, 0.1f,
				() => gasPriceSection.AnimateGraphicAndScale(0f, 0f, 0.1f,
				() => transactionSpeedSection.AnimateGraphicAndScale(1f, 1f, 0.1f,
				() => Animating = false)));
		}
	}

	private void MaxClicked()
	{
		bool maxToggledOn = maxToggle.transform.GetComponent<ToggleAnimator>().IsToggledOn;

		amountSection.transform.GetChild(2).GetComponent<TMP_InputField>().interactable = maxToggledOn ? false : true;

		amountSection.transform.GetChild(2).GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = maxToggledOn ? tokenSection.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text : "Enter amount...";
		amountSection.transform.GetChild(2).GetComponent<TMP_InputField>().text = maxToggledOn ? "" : tokenSection.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
	}
}
