using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class which contains a series of utility methods relating to input fields.
/// </summary>
public static class InputFieldUtils
{

    /// <summary>
    /// Moves the current selection to the next input field in a list.
    /// </summary>
    /// <param name="inputFields"> The list of input fields to use for the switching. </param>
    public static void MoveToNextInputField(this IList<InputField> inputFields)
    {
        var system = EventSystem.current;
        var selectedField = system.currentSelectedGameObject.GetComponent<InputField>();

        if (selectedField == null || !selectedField.interactable)
            return;

        do {
            var index = inputFields.IndexOf(selectedField) + 1;
            selectedField = inputFields[index >= inputFields.Count ? 0 : index];
        } while (!selectedField.interactable);

        selectedField.OnPointerClick(new PointerEventData(system));
        system.SetSelectedGameObject(selectedField.gameObject, new BaseEventData(system));
    }

    /// <summary>
    /// Gets the actively selected input field.
    /// </summary>
    /// <returns> The currently active input field. </returns>
    public static InputField GetActiveInputField() => EventSystem.current.currentSelectedGameObject.GetComponent<InputField>();

}
