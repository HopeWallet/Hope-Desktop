using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class VerifyingPasswordForm : UIAnimator
{

	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject text;
	[SerializeField] private GameObject loadingIcon;

	private string mainText = "Verifying Password";

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
	/// Spins the loadingIcon around in a circle
	/// </summary>
	private void Update()
	{
		loadingIcon.transform.DOLocalRotate(new Vector3(0f, 0f, -360f), 5f, RotateMode.LocalAxisAdd);
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
			() => loadingIcon.AnimateGraphicAndScale(1f, 0.75f, 0.1f, FinishedAnimating))));
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
		loadingIcon.AnimateGraphicAndScale(0f, 0f, 0.1f,
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

		if (MainText == "Verifying Password....")
			MainText = "Verifying Password";
		else
			MainText += ".";

		StartCoroutine(AddDots());
	}
}
