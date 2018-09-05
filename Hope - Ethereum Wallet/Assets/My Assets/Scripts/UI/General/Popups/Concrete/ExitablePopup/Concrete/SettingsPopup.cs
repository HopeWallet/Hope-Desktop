using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	[SerializeField] private Button downloadUpdateButton;

	protected override void Awake()
	{
		base.Awake();

		downloadUpdateButton.onClick.AddListener(() => Application.OpenURL("http://www.hopewallet.io/"));
	}

	private void OnDisable() => MoreDropdown.PopupClosed?.Invoke();
}