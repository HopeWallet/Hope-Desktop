using UnityEngine.UI;

/// <summary>
/// Class which contains extension methods for setting the text for certain ui components.
/// </summary>
public static class TextExtensions
{

    /// <summary>
    /// Sets the text of the text component if it is not null.
    /// </summary>
    /// <param name="textComponent"> The text component to set the text for. </param>
    /// <param name="text"> The text to set to the component. </param>
    public static void SetText(this Text textComponent, string text)
    {
        if (textComponent == null)
            return;

        textComponent.text = text;
    }

}