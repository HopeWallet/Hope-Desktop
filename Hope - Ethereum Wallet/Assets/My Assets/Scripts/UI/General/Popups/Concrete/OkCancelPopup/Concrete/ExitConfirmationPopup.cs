using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public sealed class ExitConfirmationPopup : OkCancelPopupComponent<ExitConfirmationPopup>
{

	public GameObject noteText;
	
	/// <summary>
	/// Closes the wallet
	/// </summary>
	public override void OkButton() => Application.Quit();

	public override void CancelButton()
	{
		DOTween.RestartAll();
		popupManager.CloseActivePopup();
	}

	/// <summary>
	/// Sets the note text to be active or not
	/// </summary>
	/// <param name="active"> Whether the note text should be active or not </param>
	public void SetDetails(bool active) => noteText.SetActive(active);
}