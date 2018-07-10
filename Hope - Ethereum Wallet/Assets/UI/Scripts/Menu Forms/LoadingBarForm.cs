using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingBarForm : UIAnimator
{

	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject text;
	[SerializeField] private GameObject loadingBar;

	private string mainText = "Loading";

	/// <summary>
	/// The main text object's string
	/// </summary>
	private string MainText
	{
		get { return mainText; }

		set
		{
			mainText = value;
			text.GetComponent<TextMeshProUGUI>().text = value;
		}
	}


	/// <summary>
	/// Starts the coroutine
	/// </summary>
	private void Start()
	{
		StartCoroutine(AddDots());
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
		dim.AnimateGraphic(1f, 0.1f,
			() => form.AnimateGraphicAndScale(1f, 1f, 0.1f,
			() => text.AnimateScaleX(1f, 0.1f,
			() => loadingBar.AnimateScaleX(1f, 0.1f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		loadingBar.AnimateScaleX(0f, 0.1f,
			() => text.AnimateScaleX(0f, 0.1f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.1f,
			() => dim.AnimateGraphic(0f, 0.1f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates the dots in the string of the text object
	/// </summary>
	/// <returns></returns>
	private IEnumerator AddDots()
	{
		yield return new WaitForSeconds(0.3f);

		if (MainText == "Loading....")
			MainText = "Loading";
		else
			MainText += ".";

		StartCoroutine(AddDots());
	}
}
