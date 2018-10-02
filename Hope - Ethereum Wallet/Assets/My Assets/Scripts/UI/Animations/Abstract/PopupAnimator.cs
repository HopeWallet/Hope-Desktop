using UnityEngine;

/// <summary>
/// The class that manages the general popup animations
/// </summary>
public abstract class PopupAnimator : UIAnimator
{
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject blur;
	[SerializeField] private GameObject popupContainer;

	private Vector2 startingPosition;

	/// <summary>
	/// Sets the current starting position given by the current mouse position at the time of the click
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
	/// Animates the basic elements of the popup into view
	/// </summary>
	protected override void AnimateIn()
	{
		popupContainer.AnimateTransform(Vector2.one, 0.2f);
		dim.AnimateGraphic(1f, 0.3f);
		blur.AnimateScale(1f, 0.2f);
		formTitle.AnimateGraphicAndScale(1f, 1f, 0.35f);
		form.AnimateGraphicAndScale(1f, 1f, 0.2f, AnimateUniqueElementsIn);
	}

	/// <summary>
	/// Animates the basic elements out of view
	/// </summary>
	protected override void AnimateOut()
	{
		popupContainer.AnimateTransform(startingPosition, 0.2f);
		form.AnimateScale(0f, 0.2f, FinishedAnimating);
		dim.AnimateGraphic(0f, 0.2f);
		blur.AnimateScale(0f, 0.2f);
	}

	/// <summary>
	/// Animate the unique elements of the form into view
	/// </summary>
	protected abstract void AnimateUniqueElementsIn();
}
