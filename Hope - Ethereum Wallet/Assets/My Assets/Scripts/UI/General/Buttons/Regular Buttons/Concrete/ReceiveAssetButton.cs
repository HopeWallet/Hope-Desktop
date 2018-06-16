using Zenject;
/// <summary>
/// Class which is used for the Receive button, which opens the popup.
/// </summary>
public class ReceiveAssetButton : ButtonBase
{

    private PopupManager popupManager;

    /// <summary>
    /// Adds the PopupManager dependency to this component.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    [Inject]
    public void Construct(PopupManager popupManager) => this.popupManager = popupManager;

    /// <summary>
    /// Opens the ReceiveAssetPopup.
    /// </summary>
    public override void ButtonLeftClicked() => popupManager.GetPopup<ReceiveAssetPopup>();

}
