using UnityEngine;
using UnityEngine.UI;

public class ChooseWalletForm : FormAnimation
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject ledgerButton;
	[SerializeField] private GameObject hopeButton;

	/// <summary>
	/// Initializes the necessary variables that haven't already been initialized in the inspector
	/// </summary>
	protected override void InitializeElements()
	{
		hopeButton.GetComponent<Button>().onClick.AddListener(HopeButtonClicked);
		ledgerButton.GetComponent<Button>().onClick.AddListener(LedgerButtonClicked);
		form.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(BackButtonClicked);
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
			() => ledgerButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => hopeButton.AnimateGraphicAndScale(1f, 1f, 0.2f, FinishedAnimatingIn))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		hopeButton.AnimateGraphicAndScale(0f, 0.1f, 0.2f);
		ledgerButton.AnimateGraphicAndScale(0f, 0.1f, 0.2f);
		title.AnimateGraphicAndScale(0f, 0.1f, 0.2f,
			() => form.AnimateGraphicAndScale(0f, 0.1f, 0.2f, FinishedAnimatingOut));		
	}

	/// <summary>
	/// Hope wallet button has been clicked
	/// </summary>
	private void HopeButtonClicked()
	{
		DisableMenu();
		// GO TO NEXT FORM
	}

	/// <summary>
	/// Ledger wallet button has been clicked
	/// </summary>
	private void LedgerButtonClicked()
	{
		DisableMenu();
		// GO TO NEXT FORM
	}

	/// <summary>
	/// Back button has been clicked
	/// </summary>
	private void BackButtonClicked()
	{
		DisableMenu();
		// GO TO PREVIOUS FORM
	}
}
