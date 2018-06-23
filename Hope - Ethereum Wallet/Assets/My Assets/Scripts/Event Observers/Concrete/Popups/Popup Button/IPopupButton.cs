using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Interface for classes to inherit when they are popup buttons which should be disabled if the mouse is clicked away from the button.
/// </summary>
public interface IPopupButton : IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Whether the mouse pointer is currently over the button.
    /// </summary>
    bool PointerEntered { get; }

    /// <summary>
    /// The popup gameobject.
    /// </summary>
    GameObject PopupObject { get; }
}