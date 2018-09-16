using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIAnimator : MonoBehaviour
{
	[SerializeField] private bool animateOnEnable;

	[SerializeField] private GameObject blocker;
	[SerializeField] private GameObject dim;
	[SerializeField] protected GameObject form;
	[SerializeField] private GameObject blur;
	[SerializeField] protected GameObject formTitle;
	[SerializeField] protected GameObject popupContainer;

	protected Vector2 startingPosition;

	protected Action onAnimationFinished;

	private bool animating;

	public bool Animating
	{
		get { return animating; }
		protected set { ChangeAnimationState(value); }
	}

	/// <summary>
	/// Sets the starting position if it is a popup, and starts to animate in
	/// </summary>
	private void OnEnable()
	{
		if (popupContainer != null)
		{
			Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			Vector2 updatedPosition = new Vector2(GetUpdatedValue(Screen.width / 2, currentMousePosition.x), GetUpdatedValue(Screen.height / 2, currentMousePosition.y));

			popupContainer.transform.localPosition = updatedPosition;
			startingPosition = updatedPosition;
		}

		if (animateOnEnable)
			AnimateEnable();
	}

	/// <summary>
	/// Gets the updates position of the form compared to the mouse click
	/// </summary>
	/// <param name="ScreenDimensionInHalf"> The screen dimension divided by 2 </param>
	/// <param name="currentPosition"> The current position of the mouse on a specific axis </param>
	/// <returns></returns>
	private float GetUpdatedValue(float ScreenDimensionInHalf, float currentPosition)
	{
		return currentPosition > ScreenDimensionInHalf ? currentPosition - ScreenDimensionInHalf : (ScreenDimensionInHalf - currentPosition) * -1f;
	}

	/// <summary>
	/// Animates the form in
	/// </summary>
	/// <param name="onAnimationFinished"> The action to be called after the animation finishes </param>
	public void AnimateEnable(Action onAnimationFinished = null)
	{
		this.onAnimationFinished = onAnimationFinished;

		ChangeAnimationState(true);
		AnimateBasicElements(true);
	}

	/// <summary>
	/// Animates the form out
	/// </summary>
	/// <param name="onAnimationFinished"> The action to be called after the animation finishes </param>
	public void AnimateDisable(Action onAnimationFinished = null)
	{
		this.onAnimationFinished = onAnimationFinished;

		ChangeAnimationState(true);
		AnimateBasicElements(false);
	}

	/// <summary>
	/// Animates the basic elements of the form, such as the dim, blur, form and title
	/// </summary>
	/// <param name="animateIn"> Whether animating the elements in or out </param>
	protected void AnimateBasicElements(bool animateIn)
	{
		if (popupContainer != null)
			popupContainer.AnimateTransform(animateIn ? Vector2.zero : startingPosition, 0.2f);

		float endValue = animateIn ? 1f : 0f;

		if (dim != null) dim.AnimateGraphic(endValue, 0.2f);

		if (blur != null) blur.AnimateScale(endValue, 0.2f);

		if (formTitle != null) formTitle.AnimateGraphicAndScale(1f, 1f, animateIn ? 0.35f : 0.2f);

		if (form != null)
			form.AnimateGraphicAndScale(1f, endValue, 0.2f, () => OnCompleteAction(animateIn));
		else
			OnCompleteAction(animateIn);
	}

	/// <summary>
	/// On the completion of the elements finishing the animation, it calls other methods to either finish the animation,
	/// or animate the unique elements of the form.
	/// </summary>
	/// <param name="animateIn"></param>
	private void OnCompleteAction(bool animateIn)
	{
		if (animateIn)
		{
			AnimateUniqueElementsIn();
		}
		else
		{
			if (popupContainer == null)
				ResetElementValues();
			else
				FinishedAnimating();
		}
	}

	/// <summary>
	/// Animate the unique elements of the form in
	/// </summary>
	protected abstract void AnimateUniqueElementsIn();

	/// <summary>
	/// Resets the unique elements of the form back to the starting positions
	/// </summary>
	protected virtual void ResetElementValues() { }
	
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
