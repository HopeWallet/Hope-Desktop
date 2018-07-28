using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class UIAnimatorExtensions
{

	public static void AnimateGraphicAndScale(this GameObject gameObject, float alpha, float scale, float duration, TweenCallback callback = null)
	{
		gameObject.AnimateGraphic(alpha, duration);
		gameObject.AnimateScaleX(scale, duration);
		gameObject.AnimateScaleY(scale, duration, callback);
	}

	public static void AnimateGraphic(this GameObject gameObject, float endValue, float duration, TweenCallback callback = null)
	{
		gameObject.GetComponent<Graphic>().DOFade(endValue, duration).OnComplete(() => callback?.Invoke());
	}

	public static void AnimateScaleX(this GameObject gameObject, float endValue, float duration, TweenCallback callback = null)
	{
		gameObject.transform.DOScaleX(endValue, duration).OnComplete(() => callback?.Invoke());
	}

	public static void AnimateScaleY(this GameObject gameObject, float endValue, float duration, TweenCallback callback = null)
	{
		gameObject.transform.DOScaleY(endValue, duration).OnComplete(() => callback?.Invoke());
	}

	public static void AnimateTransformY(this GameObject gameObject, float endValue, float duration)
	{
		gameObject.transform.DOLocalMoveY(endValue, duration);
	}

	public static void AnimateTransformX(this GameObject gameObject, float endValue, float duration)
	{
		gameObject.transform.DOLocalMoveX(endValue, duration);
	}

	public static void AnimateMaterialBlur(this Image image, float incrementBlur, float duration)
	{
		Material imageMaterial = image.material;

		imageMaterial.DOFloat(imageMaterial.GetFloat("_Size") + incrementBlur, "_Size", duration);
	}
}
