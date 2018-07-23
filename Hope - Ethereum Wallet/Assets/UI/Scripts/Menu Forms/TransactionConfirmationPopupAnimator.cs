using UnityEngine;
using DG.Tweening;
using TMPro;

public class TransactionConfirmationPopupAnimator : UIAnimator
{

	[SerializeField] private GameObject timerText;

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
		AnimateTimerText(3);
	}

	private void AnimateTimerText(int num)
	{
		timerText.GetComponent<TextMeshProUGUI>().text = num.ToString();

		timerText.AnimateScaleX(1f, 0.1f,
			() => timerText.AnimateScaleX(0f, 0.9f));

		if (num == 1)
		{
			timerText.AnimateScaleY(1f, 0.1f,
				() => timerText.AnimateScaleY(0f, 0.9f));
		}

		else
		{
			timerText.AnimateScaleY(1f, 0.1f,
				() => timerText.AnimateScaleY(0f, 0.9f,
				() => AnimateTimerText(--num)));
		}
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{
	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{
	}
}
