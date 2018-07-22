using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class which has extension methods for ui selectable elements.
/// </summary>
public static class SelectableExtensions
{

    /// <summary>
    /// Moves the current selected component to the next in a list of <see cref="Selectable"/>.
    /// </summary>
    /// <param name="selectables"> The list of <see cref="Selectable"/> components to use to determine which to switch to. </param>
    public static void MoveToNextSelectable(this IList<Selectable> selectables)
    {
        if (selectables.Count == 0)
            return;

        var system = EventSystem.current;
        var selectable = system.currentSelectedGameObject?.GetComponent<Selectable>();

        if (selectable?.interactable != true)
        {
            SelectSelectable(selectables[0]);
            return;
        }

        do
        {
            var index = selectables.IndexOf(selectable) + 1;
            selectable = selectables[index >= selectables.Count ? 0 : index];
        } while (!selectable.interactable);

        SelectSelectable(selectable);
    }

    /// <summary>
    /// Selects a certain <see cref="Selectable"/> component.
    /// </summary>
    /// <param name="selectable"> The <see cref="Selectable"/> component to select. </param>
    public static void SelectSelectable(this Selectable selectable)
    {
        var system = EventSystem.current;

        selectable.Select();
        system.SetSelectedGameObject(selectable.gameObject, new BaseEventData(system));

        SelectAsInputField(selectable, system);
    }

    /// <summary>
    /// Selects the <see cref="Selectable"/> as an input field if it has an <see cref="IPointerClickHandler"/>.
    /// </summary>
    /// <param name="selectable"> The <see cref="Selectable"/> component to select. </param>
    /// <param name="system"> The current <see cref="EventSystem"/>. </param>
    private static void SelectAsInputField(Selectable selectable, EventSystem system) => (selectable as IPointerClickHandler)?.OnPointerClick(new PointerEventData(system));

}