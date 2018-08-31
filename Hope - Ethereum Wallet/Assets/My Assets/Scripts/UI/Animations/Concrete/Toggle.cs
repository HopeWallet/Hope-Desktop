using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class Toggle : MonoBehaviour
{

	[SerializeField] private GameObject toggleBackground;
	[SerializeField] private GameObject toggleCircle;

	private Action toggleClick;

	public bool IsToggledOn { get; set; }

	/// <summary>
	/// Sets the button listeners
	/// </summary>
	private void Awake()
	{
		toggleBackground.GetComponent<Button>().onClick.AddListener(ToggleClicked);
		toggleCircle.GetComponent<Button>().onClick.AddListener(ToggleClicked);
	}

	/// <summary>
	/// Sets the toggle click action if it is currently null
	/// </summary>
	/// <param name="action"> The given Action </param>
    public void AddToggleListener(Action action)
    {
        if (toggleClick == null)
            toggleClick = action;
        else
            toggleClick += action;
    }

	/// <summary>
	/// Animates the Circle over to the left or right, and animates the colors of the circle and background image
	/// </summary>
	private void ToggleClicked()
	{
		IsToggledOn = !IsToggledOn;
		AnimateImages();
	}

	/// <summary>
	/// Animates the UI visuals
	/// </summary>
	public void AnimateImages()
	{
		toggleCircle.AnimateTransformX(IsToggledOn ? 10f : -10f, 0.1f);
		toggleCircle.GetComponent<Image>().DOColor(IsToggledOn ? UIColors.Green : UIColors.LightGrey, 0.1f);
		toggleBackground.GetComponent<Image>().DOColor(IsToggledOn ? UIColors.Green : UIColors.LightGrey, 0.1f);

		toggleClick?.Invoke();
	}

	/// <summary>
	/// Sets the toggle interactable and animates a color change
	/// </summary>
	/// <param name="interactable"> Whether the toggle is interactable or not </param>
	public void SetInteractable(bool interactable)
	{
		toggleBackground.GetComponent<Button>().interactable = interactable;
		toggleCircle.GetComponent<Button>().interactable = interactable;

		toggleBackground.AnimateColor(interactable ? UIColors.LightGrey : UIColors.DarkGrey, 0.1f);
		toggleCircle.AnimateColor(interactable ? UIColors.LightGrey : UIColors.DarkGrey, 0.1f);
	}
}
