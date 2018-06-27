using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class MenuAnimator
{

	private readonly static Dictionary<GameObject, List<Component>> animatedComponents = new Dictionary<GameObject, List<Component>>();

	public static void AnimateGraphicAndScale(this GameObject gameObject, float alpha, float scale, float duration, TweenCallback callback = null)
	{
		gameObject.AnimateGraphic(alpha, duration);
		gameObject.AnimateScaleX(scale, duration);
		gameObject.AnimateScaleY(scale, duration, callback);
	}

	public static void AnimateGraphic(this GameObject gameObject, float endValue, float duration, TweenCallback callback = null)
	{
		gameObject.GetAnimationComponent<Graphic>().DOFade(endValue, duration).OnComplete(() => callback?.Invoke());
	}

	public static void AnimateScaleX(this GameObject gameObject, float endValue, float duration, TweenCallback callback = null)
	{
		gameObject.GetAnimationComponent<Transform>().DOScaleX(endValue, duration).OnComplete(() => callback?.Invoke());
	}

	public static void AnimateScaleY(this GameObject gameObject, float endValue, float duration, TweenCallback callback = null)
	{
		gameObject.GetAnimationComponent<Transform>().DOScaleY(endValue, duration).OnComplete(() => callback?.Invoke());
	}

	public static T GetAnimationComponent<T>(this GameObject gameObject) where T : Component
	{
		if (!animatedComponents.ContainsKey(gameObject))
			animatedComponents.Add(gameObject, new List<Component>());

		if (animatedComponents[gameObject].OfType<T>().Count() == 0)
			animatedComponents[gameObject].Add(gameObject.GetComponent<T>());

		return animatedComponents[gameObject].OfType<T>().FirstOrDefault();
	}

}
