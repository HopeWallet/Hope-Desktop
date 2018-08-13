using System;
using UnityEngine;

public abstract class UIAnimator : MonoBehaviour
{
	[SerializeField] private bool animateOnEnable;

	[SerializeField] private GameObject blocker;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject blur;
	[SerializeField] protected GameObject formTitle;
	[SerializeField] protected GameObject popupContainer;

	protected Vector2 startingPosition;

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
		AnimateBasicElements(true);
	}

	public void AnimateDisable(Action onAnimationFinished = null)
	{
		this.onAnimationFinished = onAnimationFinished;

		ChangeAnimationState(true);
		AnimateUniqueElementsOut();
	}

	protected void AnimateBasicElements(bool animateIn)
	{
		if (popupContainer != null)
			popupContainer.AnimateTransform(animateIn ? Vector2.zero : startingPosition, 0.2f);

		float endValue = animateIn ? 1f : 0f;

		if (dim != null) dim.AnimateGraphic(endValue, 0.2f);
		if (blur != null) blur.AnimateScale(endValue, 0.2f);
		if (formTitle != null) formTitle.AnimateGraphicAndScale(animateIn ? 0.85f : 0f, endValue, animateIn ? 0.3f : 0.2f);

		if (form != null)
		{
			form.AnimateGraphicAndScale(endValue, endValue, 0.2f, () => { if (animateIn) AnimateUniqueElementsIn(); else FinishedAnimating(); });
		}
		else
		{
			if (animateIn)
				AnimateUniqueElementsIn();
			else
				FinishedAnimating();
		}
	}

	protected abstract void AnimateUniqueElementsIn();

	protected abstract void AnimateUniqueElementsOut();

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
