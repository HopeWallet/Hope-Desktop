using System;
using UnityEngine;

public abstract class UIAnimator : MonoBehaviour
{
	[SerializeField] private bool animateOnEnable;

	[SerializeField] private GameObject blocker;
	[SerializeField] protected GameObject formTitle;

	protected Action onAnimationFinished;

	private bool animating;

	public bool Animating
	{
		get { return animating; }
		protected set { ChangeAnimationState(value); }
	}

	/// <summary>
	/// Starts to animate in
	/// </summary>
	private void OnEnable() => AnimateEnable();

	/// <summary>
	/// Animates the form in
	/// </summary>
	/// <param name="onAnimationFinished"> The action to be called after the animation finishes </param>
	public void AnimateEnable(Action onAnimationFinished = null)
	{
		this.onAnimationFinished = onAnimationFinished;

		ChangeAnimationState(true);
		AnimateIn();
	}

	/// <summary>
	/// Animates the form out
	/// </summary>
	/// <param name="onAnimationFinished"> The action to be called after the animation finishes </param>
	public void AnimateDisable(Action onAnimationFinished = null)
	{
		this.onAnimationFinished = onAnimationFinished;

		ChangeAnimationState(true);
		AnimateOut();
	}

	/// <summary>
	/// Animates the elements of the form into view
	/// </summary>
	protected abstract void AnimateIn();

	/// <summary>
	/// Animates the elements of the form out of view
	/// </summary>
	protected abstract void AnimateOut();

	/// <summary>
	/// Changes the animation state and calls the onAnimationFinished method
	/// </summary>
	protected void FinishedAnimating()
	{
		ChangeAnimationState(false);
		onAnimationFinished?.Invoke();
	}

	/// <summary>
	/// Changes the animation state
	/// </summary>
	/// <param name="state"> Whether an animation is currently underway or not </param>
	private void ChangeAnimationState(bool state)
	{
		if (blocker != null)
			blocker.SetActive(state);

		animating = state;
	}
}
