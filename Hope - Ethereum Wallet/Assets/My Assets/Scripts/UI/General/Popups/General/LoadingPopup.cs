using UnityEngine.UI;

/// <summary>
/// Class which is used to display a loading popup on the screen.
/// </summary>
public class LoadingPopup : FactoryPopup<LoadingPopup>
{

    public Text loadingText;

    /// <summary>
    /// Sets the text of the loading popup.
    /// The text will be displayed in between "Loading" and "..."
    /// </summary>
    /// <param name="text"> The text displayed in the middle of the message. </param>
    /// <param name="startingText"> The text displayed at the start of the message. </param>
    /// <param name="endingText"> The text displayed at the end of the message. </param>
    public void SetLoadingText(string text, string startingText = "Loading ", string endingText = "...")
    {
        loadingText.text = startingText + text + endingText;
    }

}