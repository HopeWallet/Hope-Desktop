using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingIcon : MonoBehaviour
{
	private Image image;

	private readonly Color LIGHT_COLOR = new Color(1f, 1f, 1f);
	private readonly Color GRAY_COLOR = new Color(0.7f, 0.7f, 0.7f);

	/// <summary>
	/// Sets randomized variables
	/// </summary>
	private void Start()
	{
		image = transform.GetComponent<Image>();
		image.color = GRAY_COLOR;
		AnimateColor(true);
	}

	/// <summary>
	/// Animates the icon in a circle
	/// </summary>
	private void Update()
	{
		if (transform.gameObject.activeInHierarchy) transform.DOLocalRotate(new Vector3(0f, 0f, -360f), 3f, RotateMode.LocalAxisAdd);
	}

	/// <summary>
	/// Animates the image's color to a white or a gray
	/// </summary>
	/// <param name="gray"> Checks what the current color state is </param>
	private void AnimateColor(bool isGray)
	{
		image.DOColor(isGray ? LIGHT_COLOR : GRAY_COLOR, 1f).OnComplete(() => AnimateColor(!isGray));
	}
}
