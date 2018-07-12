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
    /// Whether the LoadingTextAnimator should be stopped.
    /// </summary>
    public bool Stop { get; set; }
    
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
    private void OnEnable()
    {
        StartCoroutine(AddDotsToText());
    }

    /// <summary>
    /// Animates the dots in the string of the text object
    /// </summary>
    /// <returns></returns>
    private IEnumerator AddDotsToText()
    {
        yield return new WaitForSeconds(waitTime);

        string currentText = textMeshPro.text;

        int firstIndex = currentText.IndexOf('.');
        int lastIndex = currentText.LastIndexOf('.');

        bool needsMoreDots = firstIndex == -1 || lastIndex - firstIndex < LIMIT;

        textMeshPro.text = needsMoreDots ? currentText + "." : currentText.TrimEnd('.');
        waitTime = needsMoreDots ? waitTime + 0.05f : 0.2f;

        if (!Stop)
            StartCoroutine(AddDotsToText());
        else
            textMeshPro.text = textMeshPro.text.TrimEnd('.') + ".";
    }
}
