using System.Collections;
using UnityEngine;
using TMPro;

public class LoadingText : MonoBehaviour
{

	private TextMeshProUGUI textMeshPro;

    private GameObject loadingTextObj;

    private string baseText,
                   endText,
                   textString;

	private float waitTime = 0.2f;

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
	}

	private void Start()
	{
        loadingTextObj = gameObject;
        textMeshPro = transform.GetComponent<TextMeshProUGUI>();
        baseText = textMeshPro.text + ".";
        endText = baseText + "...";
        textString = baseText;
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

		StartCoroutine(AddDotsToText());
	}
}
