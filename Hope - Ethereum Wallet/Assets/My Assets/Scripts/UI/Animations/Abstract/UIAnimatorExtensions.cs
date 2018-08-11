using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class UIAnimatorExtensions
{

	/// <summary>
	/// Animates the graphic and scale X and Y
	/// </summary>
	/// <param name="gameObject"> The GameObject being animated </param>
	/// <param name="alpha"> The alpha endValue </param>
	/// <param name="scale"> The scale endValue </param>
	/// <param name="duration"> The duration of the animation </param>
	/// <param name="callback"> The method to call after animation is complete </param>
	public static void AnimateGraphicAndScale(this GameObject gameObject, float alpha, float scale, float duration, TweenCallback callback = null)
	{
		gameObject.AnimateGraphic(alpha, duration);
		gameObject.AnimateScale(scale, duration, callback);
	}

	/// <summary>
	/// Animates the graphic component of the image's color
	/// </summary>
	/// <param name="gameObject"> The GameObject being animated </param>
	/// <param name="endValue"> The endvalue to reach </param>
	/// <param name="duration"> The duration of the animation </param>
	/// <param name="callback"> The method to call after the animation is complete </param>
	public static void AnimateGraphic(this GameObject gameObject, float endValue, float duration, TweenCallback callback = null)
	{
		gameObject.GetComponent<Graphic>().DOFade(endValue, duration).OnComplete(() => callback?.Invoke());
	}

	/// <summary>
	/// Animates the scale X and Y
	/// </summary>
	/// <param name="gameObject"> The GameObject being animated </param>
	/// <param name="endValue"> The endvalue to reach </param>
	/// <param name="duration"> The duration of the animation </param>
	/// <param name="callback"> The method to call after the animation is complete </param>
	public static void AnimateScale(this GameObject gameObject, float endValue, float duration, TweenCallback callback = null)
	{
		gameObject.AnimateScaleX(endValue, duration);
		gameObject.AnimateScaleY(endValue, duration, callback);
	}

	/// <summary>
	/// Animates the scale X
	/// </summary>
	/// <param name="gameObject"> The GameObject being animated </param>
	/// <param name="endValue"> The endvalue to reach </param>
	/// <param name="duration"> The duration of the animation </param>
	/// <param name="callback"> The method to call after the animation is complete </param>
	public static void AnimateScaleX(this GameObject gameObject, float endValue, float duration, TweenCallback callback = null)
	{
		gameObject.transform.DOScaleX(endValue, duration).OnComplete(() => callback?.Invoke());
	}

	/// <summary>
	/// Animates the scale Y
	/// </summary>
	/// <param name="gameObject"> The GameObject being animated </param>
	/// <param name="endValue"> The endvalue to reach </param>
	/// <param name="duration"> The duration of the animation </param>
	/// <param name="callback"> The method to call after the animation is complete </param>
	public static void AnimateScaleY(this GameObject gameObject, float endValue, float duration, TweenCallback callback = null)
	{
		gameObject.transform.DOScaleY(endValue, duration).OnComplete(() => callback?.Invoke());
	}

	/// <summary>
	/// Animates a move in the X axis
	/// </summary>
	/// <param name="gameObject"> The GameObject being animated </param>
	/// <param name="endValue"> The endvalue to reach </param>
	/// <param name="duration"> The duration of the animation </param>
	public static void AnimateTransformX(this GameObject gameObject, float endValue, float duration)
	{
		gameObject.transform.DOLocalMoveX(endValue, duration);
	}

	/// <summary>
	/// Animates a move in the Y axis
	/// </summary>
	/// <param name="gameObject"> The GameObject being animated </param>
	/// <param name="endValue"> The endvalue to reach </param>
	/// <param name="duration"> The duration of the animation </param>
	public static void AnimateTransformY(this GameObject gameObject, float endValue, float duration)
	{
		gameObject.transform.DOLocalMoveY(endValue, duration);
	}

	public static void AnimateMaterialBlur(this Image image, float scale, float duration, TweenCallback callback = null)
	{

	}
}
