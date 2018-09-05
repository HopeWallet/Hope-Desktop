using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : FactoryPopup<SettingsPopup>
{
	[SerializeField] private Button downloadUpdateButton;


	protected override void Awake()
	{
		base.Awake();

		downloadUpdateButton.onClick.AddListener(() => Application.OpenURL("http://www.hopewallet.io/"));
	}
}