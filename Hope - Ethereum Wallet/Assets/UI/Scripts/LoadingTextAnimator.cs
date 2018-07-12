using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Class which animates loading text.
/// </summary>
public class LoadingTextAnimator : MonoBehaviour
{

    public string startingText;
    public string finalText;

    private TextMeshProUGUI textMeshPro;

    private GameObject loadingTextObj;

    private string baseText,
                   endText,
                   textString;

    private float waitTime = 0.2f;

    /// <summary>
    /// Whether the LoadingTextAnimator should be stopped.
    /// </summary>
    public bool Stop { get; set; }

    /// <summary>
    /// The main text object's string
    /// </summary>
    private string TextString
    {
        get { return textString; }

        set
        {
            textString = value;
            textMeshPro.text = value;
        }
    }
    
    /// <summary>
    /// Initializes elements.
    /// </summary>
    private void Awake()
    {
        loadingTextObj = gameObject;
        textMeshPro = transform.GetComponent<TextMeshProUGUI>();
        baseText = startingText + ".";
        endText = baseText + "...";
        textString = baseText;
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

        TextString = TextString == endText ? baseText : TextString + ".";
        waitTime = TextString == endText ? 0.2f : waitTime + 0.05f;

        if (Stop)
            textMeshPro.text = finalText;
        else
            StartCoroutine(AddDotsToText());

    }
}
