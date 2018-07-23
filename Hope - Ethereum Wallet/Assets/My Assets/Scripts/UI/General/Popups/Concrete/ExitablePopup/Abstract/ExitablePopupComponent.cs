using UnityEngine.UI;

/// <summary>
/// Class which represents a popup with displays information and has an exit button.
/// </summary>
/// <typeparam name="T"> The type of popup. </typeparam>
public abstract class ExitablePopupComponent<T> : FactoryPopup<T> where T : FactoryPopup<T>
{

    public Button exitButton;

    /// <summary>
    /// Initializes the exit button of the ExitablePopup.
    /// </summary>
    private void Start()
    {
        exitButton.onClick.AddListener(ExitButton);
        OnStart();
    }

    /// <summary>
    /// Called when the exit button is clicked.
    /// </summary>
    public void ExitButton()
    {
        ExitPopup();
        OnExitClicked();
    }

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
    /// Method called when the Exit button is clicked.
    /// Used for parent classes.
    /// </summary>
    protected virtual void OnExitClicked() { }

}
