using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class which is a base where all unique interactable buttons will inherit.
/// </summary>
public abstract class ButtonBase : MonoBehaviour, IPointerClickHandler
{

    private Button button;

    /// <summary>
    /// The actual Button component of object.
    /// </summary>
    public Button Button { get { if (button == null) button = GetComponent<Button>(); return button; } private set { button = value; } }

    /// <summary>
    /// Initializes the button by getting the required components and adding the on click listener.
    /// </summary>
    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        OnAwake();
    }

    /// <summary>
    /// Called when the component starts.
    /// </summary>
    protected virtual void OnAwake() { }

    /// <summary>
    /// Called when the pointer clicks on this object.
    /// </summary>
    /// <param name="eventData"> The data of the click event. </param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (button == null)
            return;

        if (button.interactable && eventData.button == PointerEventData.InputButton.Left)
            ButtonLeftClicked();
        else if (eventData.button == PointerEventData.InputButton.Right)
            ButtonRightClicked();
        else
            ButtonMiddleClicked();
    }

    /// <summary>
    /// Called when the button is left clicked.
    /// </summary>
    public virtual void ButtonLeftClicked() { }

    /// <summary>
    /// Called when the button is right clicked.
    /// </summary>
    public virtual void ButtonRightClicked() { }

    /// <summary>
    /// Called when the button is middle clicked.
    /// </summary>
    public virtual void ButtonMiddleClicked() { }

}
