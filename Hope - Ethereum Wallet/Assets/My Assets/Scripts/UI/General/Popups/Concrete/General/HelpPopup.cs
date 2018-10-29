using UnityEngine;

public class HelpPopup : ExitablePopupComponent<HelpPopup>
{
	/// <summary>
	/// Calls the PopupClosed action
	/// </summary>
	private void OnDestroy() => MoreDropdown.PopupClosed?.Invoke();
}
