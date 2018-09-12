using UnityEngine;

/// <summary>
/// Base class for all popups.
/// </summary>
public abstract class PopupBase : MonoBehaviour
{
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

}