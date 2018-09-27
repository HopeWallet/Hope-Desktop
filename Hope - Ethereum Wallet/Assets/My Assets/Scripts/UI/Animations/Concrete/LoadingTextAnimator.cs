using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Class which animates loading text.
/// </summary>
public class LoadingTextAnimator : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    private float waitTime = 0.2f;

    private const int LIMIT = 3;
    
    /// <summary>
    /// Initializes elements.
    /// </summary>
    private void Awake()
    {
        textMeshPro = transform.GetComponent<TextMeshProUGUI>();
    }

	/// <summary>
	/// Starts the coroutine.
	/// </summary>
	private void OnEnable() => StartCoroutine(AddDotsToText());

	/// <summary>
	/// Trims the extra periods right as it is disabled
	/// </summary>
	private void OnDisable() => textMeshPro.text = textMeshPro.text.TrimEnd('.');

	/// <summary>
	/// Animates the dots in the string of the text object
	/// </summary>
	/// <returns></returns>
	private IEnumerator AddDotsToText()
    {
        yield return new WaitForSeconds(waitTime);

		if (!this.enabled)
			yield break;

        string currentText = textMeshPro.text;

        int firstIndex = currentText.IndexOf('.');
        int lastIndex = currentText.LastIndexOf('.');

        bool needsMoreDots = firstIndex == -1 || lastIndex - firstIndex < LIMIT;

		textMeshPro.text = needsMoreDots ? currentText + "." : currentText.TrimEnd('.');
		waitTime = needsMoreDots ? waitTime + 0.05f : 0.2f;

		StartCoroutine(AddDotsToText()); 
    }
}
