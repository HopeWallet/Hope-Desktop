using TMPro;
using UnityEngine;

/// <summary>
/// The tooltip popup
/// </summary>
public sealed class TooltipPopup : FactoryPopup<TooltipPopup>
{
	[SerializeField] private TextMeshProUGUI title;
	[SerializeField] private TextMeshProUGUI body;
	[SerializeField] private GameObject infoIcon;
	[SerializeField] private GameObject errorIcon;

	/// <summary>
	/// Sets the UI elements of the info popup
	/// </summary>
	/// <param name="titleText"> The title text string being set </param>
	/// <param name="bodyText"> The body text string being set </param>
	/// <param name="iconPosition"> The icon so that the popup can animate next to it </param>
	/// <param name="itemWidthHalf"> The width of the item being hovered over </param>
	/// <param name="showInfoIcon"> Whether the info icon should be showed or not </param>
	public void SetUIElements(string titleText, string bodyText, Vector2 iconPosition, float itemWidthHalf, bool showInfoIcon)
	{
		transform.position = new Vector2(iconPosition.x + itemWidthHalf + 3f, iconPosition.y);
		title.text = titleText;
		body.text = bodyText;

		infoIcon.SetActive(showInfoIcon);
		errorIcon.SetActive(!showInfoIcon);
	}
}
