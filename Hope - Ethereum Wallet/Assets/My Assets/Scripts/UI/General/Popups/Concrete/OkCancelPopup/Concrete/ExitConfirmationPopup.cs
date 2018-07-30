using UnityEngine;

public sealed class ExitConfirmationPopup : OkCancelPopupComponent<ExitConfirmationPopup>
{

	public GameObject noteText;

	protected override void OnOkClicked() => Application.Quit();

	/// <summary>
	/// Sets the note text to be active or not
	/// </summary>
	/// <param name="active"></param>
	public void SetNoteText(bool active) => noteText.SetActive(active);
}