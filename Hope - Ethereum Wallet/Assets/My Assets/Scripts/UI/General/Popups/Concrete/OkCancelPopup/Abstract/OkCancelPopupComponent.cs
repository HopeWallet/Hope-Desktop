using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which is a base for all popups that have the Ok/Yes and Cancel/No options, or any variation of those words.
/// </summary>
public abstract class OkCancelPopupComponent<T> : FactoryPopup<T> where T : FactoryPopup<T>
{

    public Button okButton,
              cancelButton;

    protected PopupManager popupManager;

    /// <summary>
    /// Initializes the popup by getting the button components and setting up their click events.
    /// </summary>
    private void Start()
    {
        okButton.onClick.AddListener(OkButton);
        cancelButton.onClick.AddListener(CancelButton);

        OnStart();
    }

    /// <summary>
    /// Called when the Ok button is clicked.
    /// </summary>
    public void OkButton()
    {
        ExitPopup();
        OnOkClicked();
    }

    /// <summary>
    /// Called when the Cancel button is clicked.
    /// </summary>
    public void CancelButton()
    {
        ExitPopup();
        OnCancelClicked();
    }

    /// <summary>
    /// Injects the PopupManager dependencies into this popup.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager to use. </param>
    [Inject]
    public void Construct(PopupManager popupManager) => this.popupManager = popupManager;

    /// <summary>
    /// Exits the popup by destroying this popup object.
    /// </summary>
    private void ExitPopup() => popupManager.CloseActivePopup();

    /// <summary>
    /// Method called when the script starts.
    /// Used for parent classes.
    /// </summary>
    protected virtual void OnStart() { }

    /// <summary>
    /// Method called when the Ok button is clicked.
    /// Used for parent classes.
    /// </summary>
    protected virtual void OnOkClicked() { }

    /// <summary>
    /// Method called when the Cancel button is clicked.
    /// Used for parent classes.
    /// </summary>
    protected virtual void OnCancelClicked() { }

}
