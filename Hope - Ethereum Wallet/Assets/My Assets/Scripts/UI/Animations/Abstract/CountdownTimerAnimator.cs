using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CountdownTimerAnimator : UIAnimator
{

	[SerializeField] protected GameObject timerText;
	[SerializeField] protected GameObject confirmButton;
	[SerializeField] protected GameObject cancelButton;

	/// <summary>
	/// Starts the countdown timer animation
	/// </summary>
	protected void StartTimerAnimation() => new CountdownTimer(AnimateTimerText, SetButtonInteractables, 5f, 1f).StartCountdown();

	/// <summary>
	/// Sets the text to the next number, and animates the text in, then out
	/// </summary>
	/// <param name="timeLeft"> The float of the time remaining </param>
	private void AnimateTimerText(float timeLeft)
	{
		timerText.GetComponent<TextMeshProUGUI>().text = timeLeft.ToString();

		timerText.AnimateScaleX(1f, 0.1f,
			() => timerText.AnimateScaleX(0f, 0.9f));

		timerText.AnimateScaleY(1f, 0.1f,
			() => timerText.AnimateScaleY(0f, 0.9f));
	}

	/// <summary>
	/// Sets the confirm and cancel button to interactable
	/// </summary>
	private void SetButtonInteractables() => confirmButton.GetComponent<Button>().interactable = true;
}
