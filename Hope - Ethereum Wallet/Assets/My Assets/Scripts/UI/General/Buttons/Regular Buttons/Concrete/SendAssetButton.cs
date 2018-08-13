using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class used for opening the SendAssetPopup.
/// </summary>
public class SendAssetButton : ButtonBase
{
    private PopupManager popupManager;

    /// <summary>
    /// Adds the PopupManager dependency to this button.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    [Inject]
    public void Construct(PopupManager popupManager) => this.popupManager = popupManager;

	/// <summary>
	/// Executed when the button is clicked to send the current asset.
	/// </summary>
	public override void ButtonLeftClicked()
	{
		popupManager.GetPopup<SendAssetPopup>();

		Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		Debug.Log(mousePosition.x + " " + mousePosition.y);
	}

}
