using TMPro;
using UnityEngine;

public class InfoPopup : FactoryPopup<InfoPopup>
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
	/// <param name="isInfoIcon"> Checks if the user is hovering over an info icon or error icon </param>
	/// <param name="iconPosition"> The icon so that the popup can animate next to it </param>
	public void SetUIElements(string titleText, string bodyText, bool isInfoIcon, Vector2 iconPosition)
	{
		title.text = titleText;
		body.text = bodyText;
		infoIcon.SetActive(isInfoIcon);
		errorIcon.SetActive(!isInfoIcon);
		transform.position = new Vector2(iconPosition.x + 10f, iconPosition.y);
	}
}
