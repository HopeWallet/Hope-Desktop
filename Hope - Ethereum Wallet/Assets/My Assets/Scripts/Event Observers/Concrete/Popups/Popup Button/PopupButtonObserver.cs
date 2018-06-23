using System;
using System.Linq;
using Object = UnityEngine.Object;

/// <summary>
/// Class used for observing mouse clicks outside popup buttons and closing them if they are not clicked.
/// </summary>
public class PopupButtonObserver : EventObserver<IPopupButton>, ILeftClickObservable, IRightClickObservable
{

    private Action onObjectsDestroyed;

    /// <summary>
    /// Initializes the PopupButtonObserver.
    /// </summary>
    /// <param name="mouseClickObserver"> The active MouseClickObserver. </param>
    public PopupButtonObserver(MouseClickObserver mouseClickObserver)
    {
        mouseClickObserver.SubscribeObservable(this);
    }

    /// <summary>
    /// Sets the popup close action when the current buttons are not clicked.
    /// </summary>
    /// <param name="onPopupsClosed"> The action to call once the popups are closed. </param>
    public void SetPopupCloseAction(Action onPopupsClosed) => onObjectsDestroyed = onPopupsClosed;

    /// <summary>
    /// Called when mouse left click is pressed.
    /// </summary>
    /// <param name="clickType"> The ClickType of the left click. </param>
    public void OnLeftClick(ClickType clickType) => CheckPopupButtons(clickType);

    /// <summary>
    /// Called when the mouse right click is pressed.
    /// </summary>
    /// <param name="clickType"> The ClickType of the right click. </param>
    public void OnRightClick(ClickType clickType) => CheckPopupButtons(clickType);

    /// <summary>
    /// Checks the popup buttons and closes them if none of them are hovered.
    /// </summary>
    /// <param name="clickType"> The ClickType that was pressed. </param>
    private void CheckPopupButtons(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        if (observables.Count == 0 || observables.Where(observable => observable.PointerEntered).Count() > 0)
            return;

        observables.Where(button => button.PopupObject != null).ToList().SafeForEach(obj => Object.Destroy(obj.PopupObject));
        observables.Clear();

        onObjectsDestroyed?.Invoke();
    }
}