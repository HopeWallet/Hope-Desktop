using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class which contains a series of utility methods relating to input fields.
/// </summary>
public static class InputFieldUtils
{
    private const int MAX_BALANCE_FIELD_LENGTH = 25;

    /// <summary>
    /// Restricts the field to not go over the tradable asset's decimal limit or the overall decimal limit.
    /// </summary>
    /// <param name="inputField"> The input field to change the text for. </param>
    /// <param name="decimalPlaces"> The number of decimal places to restrict the input field value to. </param>
    public static void RestrictDecimalValue(this InputField inputField, int decimalPlaces)
    {
        string amount = inputField.text;

        if (amount == null)
            return;

        amount = new string(amount.Where(c => char.IsDigit(c) || c == '.').ToArray());
        inputField.text = amount;

        var decimalIndex = amount.IndexOf(".");
        var assetDecimalLength = decimalPlaces + decimalIndex + 1;

        if (decimalPlaces == 0 && decimalIndex != -1)
            amount = amount.Substring(0, amount.Length - 1);

        var substringLength = assetDecimalLength > MAX_BALANCE_FIELD_LENGTH || decimalIndex == -1 ? MAX_BALANCE_FIELD_LENGTH : assetDecimalLength;
        if (amount.Length > substringLength)
            amount = amount.Substring(0, substringLength);

        inputField.text = amount;
    }

	/// <summary>
	/// Gets the actively selected input field.
	/// </summary>
	/// <returns> The currently active input field. </returns>
	public static InputField GetActiveInputField() => EventSystem.current.currentSelectedGameObject?.GetComponent<InputField>();
}