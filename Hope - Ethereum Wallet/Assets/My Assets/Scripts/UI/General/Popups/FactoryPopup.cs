using UnityEngine;
using Zenject;

/// <summary>
/// Class which handles the factory for ui popups.
/// </summary>
/// <typeparam name="T"> The type of the FactoryPopup. </typeparam>
public abstract class FactoryPopup<T> : MonoBehaviour where T : FactoryPopup<T>
{

    /// <summary>
    /// The animator of this popup.
    /// </summary>
    public MenuAnimator Animator { get; private set; }

    /// <summary>
    /// Gets the potential animator for this popup.
    /// </summary>
    protected virtual void Awake() => Animator = GetComponent<MenuAnimator>();

    /// <summary>
    /// Class which represents the factory for popups.
    /// </summary>
    public class Factory : Factory<T>
    {
    }
}