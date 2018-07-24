using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which is a base for all popups that have the Ok/Yes and Cancel/No options, or any variation of those words.
/// </summary>
/// <typeparam name="T"> The type of the OkCancelPopupComponent. </typeparam>
public abstract class OkCancelPopupComponent<T> : FactoryPopup<T> where T : FactoryPopup<T>
{
    [SerializeField] protected Button okButton;
    [SerializeField] protected Button cancelButton;

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
    public virtual void OkButton()
    {
        ExitPopup();
        OnOkClicked();
    }

    /// <summary>
    /// Called when the Cancel button is clicked.
    /// </summary>
    public virtual void CancelButton()
    {
        ExitPopup();
        OnCancelClicked();
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
