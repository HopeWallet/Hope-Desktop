using System;
using UnityEngine;

/// <summary>
/// Base class for all popups.
/// </summary>
public abstract class PopupBase : MonoBehaviour
{
    private event Action OnClose;

    /// <summary>
    /// The animator of this popup.
    /// </summary>
    public UIAnimator Animator { get; private set; }

    /// <summary>
    /// Whether closing of this popup should be disabled.
    /// </summary>
    public bool DisableClosing { get; set; }

    /// <summary>
    /// Gets the potential animator for this popup.
    /// </summary>
    protected virtual void Awake() => Animator = GetComponent<UIAnimator>();

    /// <summary>
    /// Invokes the OnClose event.
    /// </summary>
    protected virtual void OnDestroy() => OnClose?.Invoke();

    /// <summary>
    /// Adds an action to call when the popup closes.
    /// </summary>
    /// <param name="onCloseCallback"> The action to call once the popup closes. </param>
    /// <returns> The current popup instance. </returns>
    public void OnPopupClose(Action onCloseCallback)
    {
        OnClose += onCloseCallback;
    }
}