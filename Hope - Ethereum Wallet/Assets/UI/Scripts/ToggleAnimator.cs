using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ToggleAnimator : MonoBehaviour
{

	[SerializeField] private GameObject toggleBackground;
	[SerializeField] private GameObject toggleCircle;

	private readonly Color blueColor = new Color(0.388f, 0.694f, 1f);
	private readonly Color fadedColor = new Color(1f, 1f, 1f);

	private Action toggleClick;
	private bool isToggledOn;

	public Action ToggleClick
	{
		set { toggleClick = value; }
	}

	public bool IsToggledOn
	{
		get { return isToggledOn; }
	}

	/// <summary>
	/// Sets the button listeners
	/// </summary>
	private void Awake()
	{
		toggleBackground.GetComponent<Button>().onClick.AddListener(ToggleClicked);
		toggleCircle.GetComponent<Button>().onClick.AddListener(ToggleClicked);
	}

	/// <summary>
	/// Animates the Circle over to the left or right, and animates the colors of the circle and background image
	/// </summary>
	private void ToggleClicked()
	{
		toggleCircle.AnimateTransformX(isToggledOn ? -15f : 15f, 0.1f);
		toggleCircle.GetComponent<Image>().DOColor(isToggledOn ? fadedColor : blueColor, 0.1f);
		toggleBackground.GetComponent<Image>().DOColor(isToggledOn ? fadedColor : blueColor, 0.1f);
		isToggledOn = !isToggledOn;

		toggleClick();
	}
}
