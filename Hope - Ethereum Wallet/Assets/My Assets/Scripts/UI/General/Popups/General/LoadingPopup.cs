using TMPro;
using UnityEngine;

/// <summary>
/// Class which is used to display a loading popup on the screen.
/// </summary>
public sealed class LoadingPopup : FactoryPopup<LoadingPopup>
{

    [SerializeField]
    private TMP_Text loadingText;

    /// <summary>
    /// The text of the LoadingPopup.
    /// </summary>
    public string Text { get { return loadingText.text; } set { loadingText.text = value; } }

}