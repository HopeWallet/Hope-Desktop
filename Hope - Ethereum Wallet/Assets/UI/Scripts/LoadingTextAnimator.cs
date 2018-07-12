using System.Collections;
using UnityEngine;
using TMPro;

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

    private void Awake()
    {
        loadingTextObj = gameObject;
        textMeshPro = transform.GetComponent<TextMeshProUGUI>();
        baseText = startingText + ".";
        endText = baseText + "...";
        textString = baseText;
    }

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
