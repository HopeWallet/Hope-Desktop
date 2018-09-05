using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : FactoryPopup<SettingsPopup>
{
	[SerializeField] private Button downloadUpdateButton;

	private void Awake()
	{
		downloadUpdateButton.onClick.AddListener(() => Application.OpenURL("http://www.hopewallet.io/"));
	}
}