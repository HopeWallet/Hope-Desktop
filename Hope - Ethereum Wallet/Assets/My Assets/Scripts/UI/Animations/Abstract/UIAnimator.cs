using System;
using UnityEngine;

public abstract class UIAnimator : MonoBehaviour
{

	[SerializeField]
	private bool animateOnEnable;

	[SerializeField]
	private GameObject blocker;

	protected Action onAnimationFinished;

	private bool animating;

	private void OnEnable()
	{
		if (animateOnEnable)
			AnimateEnable();
	}

	public bool Animating
	{
		get { return animating; }
		protected set { ChangeAnimationState(value); }
	}

	public void AnimateEnable(Action onAnimationFinished = null)
	{
		this.onAnimationFinished = onAnimationFinished;

		ChangeAnimationState(true);
		AnimateIn();
	}

	public void AnimateDisable(Action onAnimationFinished = null)
	{
		this.onAnimationFinished = onAnimationFinished;

		ChangeAnimationState(true);
		AnimateOut();
	}

	protected abstract void AnimateIn();

	protected abstract void AnimateOut();

	protected void FinishedAnimating()
	{
		ChangeAnimationState(false);
		onAnimationFinished?.Invoke();
	}

	private void ChangeAnimationState(bool state)
	{
		if (blocker != null)
			blocker.SetActive(state);

		animating = state;
	}

}
