using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that manages the countdown timer for a transaction if the user has the setting enabled
/// </summary>
public abstract class CountdownTimerAnimator : PopupAnimator
{
	[SerializeField] protected GameObject timerText;
	[SerializeField] protected GameObject confirmButton;
	[SerializeField] protected GameObject cancelButton;
	[SerializeField] protected GameObject confirmText;

	/// <summary>
	/// Starts the countdown timer animation
	/// </summary>
	protected void StartTimerAnimation()
	{
		if (!confirmButton.GetComponent<Button>().interactable && !confirmText.activeInHierarchy)
			new CountdownTimer(AnimateTimerText, SetButtonInteractable, 5f, 1f).StartCountdown();
	}

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
	private void SetButtonInteractable() => confirmButton.GetComponent<Button>().interactable = true;
}
