using TMPro;
using UnityEngine;

/// <summary>
/// The info popup
/// </summary>
public sealed class InfoPopup : FactoryPopup<InfoPopup>
{
	[SerializeField] private TextMeshProUGUI title;
	[SerializeField] private TextMeshProUGUI body;

	public PopupManager PopupManager => popupManager;

	/// <summary>
	/// Sets the UI elements of the info popup
	/// </summary>
	/// <param name="titleText"> The title text string being set </param>
	/// <param name="bodyText"> The body text string being set </param>
	/// <param name="iconPosition"> The icon so that the popup can animate next to it </param>
	public void SetUIElements(string titleText, string bodyText, Vector2 iconPosition)
	{
		transform.position = new Vector2(iconPosition.x + 60f, iconPosition.y);
		title.text = titleText;
		body.text = bodyText;
	}
}
