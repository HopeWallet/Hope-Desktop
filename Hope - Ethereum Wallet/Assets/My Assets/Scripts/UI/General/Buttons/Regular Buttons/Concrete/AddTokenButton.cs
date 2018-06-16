using Zenject;

/// <summary>
/// Class which handles the AddToken button and what happens on click.
/// </summary>
public class AddTokenButton : ButtonBase
{

    private PopupManager popupManager;

    /// <summary>
    /// Injects the required dependencies into the AddTokenButton.
    /// </summary>
    /// <param name="popupManager"> The PopupManager used for displaying the AddTokenPopup. </param>
    [Inject]
    public void Construct(PopupManager popupManager) => this.popupManager = popupManager;

    /// <summary>
    /// Instantiates the popup to add a token.
    /// </summary>
    public override void ButtonLeftClicked() => popupManager.GetPopup<AddTokenPopup>();

}
