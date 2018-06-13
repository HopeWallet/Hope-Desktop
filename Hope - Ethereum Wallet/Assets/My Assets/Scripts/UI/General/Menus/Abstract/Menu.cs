using UnityEngine;
using Zenject;

/// <summary>
/// Generic class to extend for all screens in the ui that can be considered a menu.
/// </summary>
/// <typeparam name="T"> The class type of the menu. </typeparam>
public abstract class Menu<T> : Menu where T : Menu<T>
{

    protected UIManager uiManager;

    /// <summary>
    /// Adds the UIManager dependency to this menu.
    /// </summary>
    /// <param name="uiManager"> The active UIManager. </param>
    [Inject]
    public void Construct(UIManager uiManager) => this.uiManager = uiManager;

    /// <summary>
    /// Class used for creating menus dynamically.
    /// </summary>
    public class Factory : Factory<T>
    {
    }
}

/// <summary>
/// Base class for the generic menus.
/// </summary>
public abstract class Menu : MonoBehaviour
{
	[Tooltip("Destroy the menu's gameobject when it is closed.")]
	public bool DestroyWhenClosed = true;

	[Tooltip("Disable menus that are under this one in the stack.")]
	public bool DisableMenusUnderneath = true;

    /// <summary>
    /// Called when the back button is pressed.
    /// </summary>
	public abstract void OnBackPressed();
}
