using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class which contains a series of utility methods relating to input fields.
/// </summary>
public static class InputFieldUtils
{

    private const int MAX_BALANCE_FIELD_LENGTH = 30;

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
    /// Restricts the field to not go over the tradable asset's decimal limit or the overall decimal limit.
    /// </summary>
    /// <param name="inputField"> The input field to change the text for. </param>
    /// <param name="activeTradableAsset"> The active tradable asset. </param>
    public static void RestrictToBalance(this InputField inputField, TradableAsset activeTradableAsset)
    {
        var sendAmount = inputField.text;

        if (sendAmount == null)
            return;

        sendAmount = new string(sendAmount.Where(c => char.IsDigit(c) || c == '.').ToArray());
        inputField.text = sendAmount;

        var decimals = activeTradableAsset.AssetDecimals;
        var decimalIndex = sendAmount.IndexOf(".");
        var assetDecimalLength = decimals + decimalIndex + 1;

        if (decimals == 0)
            if (decimalIndex != -1)
                sendAmount = sendAmount.Substring(0, sendAmount.Length - 1);

        var substringLength = assetDecimalLength > MAX_BALANCE_FIELD_LENGTH || decimalIndex == -1 ? MAX_BALANCE_FIELD_LENGTH : assetDecimalLength;
        if (sendAmount.Length > substringLength)
            sendAmount = sendAmount.Substring(0, substringLength);

        inputField.text = sendAmount;
    }

    /// <summary>
    /// Gets the actively selected input field.
    /// </summary>
    /// <returns> The currently active input field. </returns>
    public static InputField GetActiveInputField() => EventSystem.current.currentSelectedGameObject.GetComponent<InputField>();

}
