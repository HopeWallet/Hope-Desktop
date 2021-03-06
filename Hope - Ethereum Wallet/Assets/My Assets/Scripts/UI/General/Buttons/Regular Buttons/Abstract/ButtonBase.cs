﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class which is a base where all unique interactable buttons will inherit.
/// </summary>
public abstract class ButtonBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;

    /// <summary>
    /// The actual Button component of object.
    /// </summary>
    public Button Button { get { return button ?? (button = GetComponent<Button>()) ?? (button = transform.parent.GetComponent<Button>()); } private set { button = value; } }

    /// <summary>
    /// Initializes the button by getting the required components and adding the on click listener.
    /// </summary>
    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>() ?? transform.parent.GetComponent<Button>();

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
	/// Called when the pointer hovers over this object.
	/// </summary>
	/// <param name="eventData">  The data of the event. </param>
	public void OnPointerEnter(PointerEventData eventData) => ButtonHovered(true);

	/// <summary>
	/// Called when the pointer leaves this object.
	/// </summary>
	/// <param name="eventData"> The data of the event. </param>
	public void OnPointerExit(PointerEventData eventData) => ButtonHovered(false);

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

	/// <summary>
	/// Called when the pointer hover state has been changed.
	/// </summary>
	/// <param name="mouseHovering"> Whether the mouse is hovering over this object or not. </param>
	public virtual void ButtonHovered(bool mouseHovering) { }
}
