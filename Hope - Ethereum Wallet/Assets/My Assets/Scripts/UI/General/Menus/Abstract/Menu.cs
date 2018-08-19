using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Generic class to extend for all screens in the ui that can be considered a menu.
/// </summary>
/// <typeparam name="T"> The class type of the menu. </typeparam>
public abstract class Menu<T> : Menu where T : Menu<T>
{
	protected UIManager uiManager;
	protected PopupManager popupManager;

	/// <summary>
	/// Adds the UIManager dependency to this menu.
	/// </summary>
	/// <param name="uiManager"> The active UIManager. </param>
	/// <param name="popupManager"> The active PopupManager. </param>
	[Inject]
	public void Construct(UIManager uiManager, PopupManager popupManager)
	{
		this.uiManager = uiManager;
		this.popupManager = popupManager;
	}

	/// <summary>
	/// Calls the OnBackPressed method when this menu is not animating.
	/// </summary>
	public override void GoBack()
	{
		if (!Animator.Animating)
		{
			OnBackPressed();
			backButton?.GetComponent<InteractableElement>().SetCursor(false);
		}
	}

	/// <summary>
	/// Closes the menu when the back button is pressed and the menu isn't animating.
	/// </summary>
	protected virtual void OnBackPressed() => uiManager.CloseMenu();

	/// <summary>
	/// Opens up the exit confirmation popup before closing.
	/// </summary>
	public void OnApplicationQuit()
	{
		if (popupManager.ActivePopupType != typeof(ExitConfirmationPopup))
			Application.CancelQuit();

		OpenExitConfirmationPopup();
	}

	/// <summary>
	/// Opens up the exit confirmation popup.
	/// </summary>
	protected virtual void OpenExitConfirmationPopup() => popupManager.GetPopup<ExitConfirmationPopup>()?.SetDetails(false);

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

	[SerializeField] protected Button backButton;

	/// <summary>
	/// The class responsible for animating this menu.
	/// </summary>
	public UIAnimator Animator { get; private set; }

	/// <summary>
	/// Gets the Menus animation component.
	/// </summary>
	private void Awake()
	{
		backButton?.onClick.AddListener(GoBack);
		Animator = GetComponent<UIAnimator>();
		OnAwake();
	}

	/// <summary>
	/// Called during <see cref="Awake"/>.
	/// </summary>
	protected virtual void OnAwake() { }

	/// <summary>
	/// Called when the back button is pressed.
	/// </summary>
	public abstract void GoBack();
}
