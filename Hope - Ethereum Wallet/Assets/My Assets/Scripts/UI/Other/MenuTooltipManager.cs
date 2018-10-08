using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages the hovering of the wallet logo, and visibilty of the menu tooltip
/// </summary>
public sealed class MenuTooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Action CloseMenuTooltip { get; private set; }

	[SerializeField] private GameObject menuTooltip;
	[SerializeField] private GameObject background;
	[SerializeField] private GameObject triangle;
	[SerializeField] private GameObject text;

	/// <summary>
	/// Sets the CloseMenuTooltip action
	/// </summary>
	private void Awake() => CloseMenuTooltip = () => AnimateTooltip(false);

	/// <summary>
	/// Pointer hovers over the menu tool tip or wallet logo
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData) => AnimateTooltip(true);

	/// <summary>
	/// Pointer leaves the menu tooltip and wallet logo
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData) => AnimateTooltip(false);

	/// <summary>
	/// Animates the menu tooltip
	/// </summary>
	/// <param name="animateIn"> Whether animating in or out of view </param>
	private void AnimateTooltip(bool animateIn)
	{
		if (animateIn)
			menuTooltip.SetActive(true);

		background.AnimateGraphic(animateIn ? 1f : 0f, 0.15f);
		triangle.AnimateGraphic(animateIn ? 1f : 0f, 0.15f);
		text.AnimateGraphic(animateIn ? 1f : 0f, 0.15f, () => { if (!animateIn) menuTooltip.SetActive(false); });
	}
}
