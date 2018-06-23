using System;
using System.Linq;
using Object = UnityEngine.Object;

public class PopupButtonObserver : EventObserver<IPopupButton>, ILeftClickObservable, IRightClickObservable
{

    private Action onObjectsDestroyed;

    public PopupButtonObserver(MouseClickObserver mouseClickObserver)
    {
        mouseClickObserver.SubscribeObservable(this);
    }

    public void SetPopupCloseAction(Action onPopupsClosed) => onObjectsDestroyed = onPopupsClosed;

    public void OnLeftClick(ClickType clickType) => CheckPopupButtons(clickType);

    public void OnRightClick(ClickType clickType) => CheckPopupButtons(clickType);

    private void CheckPopupButtons(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        //UnityEngine.Debug.Log(observables.Where(observable => observable.PointerEntered).Count());
        //observables.Where(obs => obs.PointerEntered).ForEach(obs => UnityEngine.Debug.Log(obs.PopupObject.name));

        if (observables.Count == 0 || observables.Where(observable => observable.PointerEntered).Count() > 0)
            return;

        observables.Where(button => button.PopupObject != null).ToList().SafeForEach(obj => Object.Destroy(obj.PopupObject));
        observables.Clear();

        onObjectsDestroyed?.Invoke();
    }
}