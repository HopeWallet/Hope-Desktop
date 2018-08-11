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
    public string Text
	{
		get { return loadingText.text; }
		set
		{
			loadingText.transform.localPosition = new Vector2(175f - (value.Length * 6.5f), loadingText.transform.localPosition.y);
			loadingText.text = value;
		}
	}

	private void OnApplicationQuit()
	{
		
	}
}