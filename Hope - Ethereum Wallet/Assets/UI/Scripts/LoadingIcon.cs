using UnityEngine;
using DG.Tweening;

public class LoadingIcon : MonoBehaviour
{

	/// <summary>
	/// Animates the icon in a circle
	/// </summary>
	private void Update()
	{
		if (transform.gameObject.activeInHierarchy) transform.DOLocalRotate(new Vector3(0f, 0f, -360f), 4f, RotateMode.LocalAxisAdd);
	}
}
