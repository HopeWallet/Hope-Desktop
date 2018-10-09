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
	/// <param name="itemWidthHalf"> The width of the item being hovered over </param>
	public void SetUIElements(string titleText, string bodyText, Vector2 iconPosition, float itemWidthHalf)
	{
		transform.position = new Vector2(iconPosition.x + itemWidthHalf, iconPosition.y);
		title.text = titleText;
		body.text = bodyText;
	}
}
