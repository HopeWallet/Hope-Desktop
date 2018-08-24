using UnityEngine;
using UnityEngine.EventSystems;

public class ListScrollbarManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	[SerializeField] private GameObject scrollBar, scrollbarHandle;

	private bool hovering;
	private bool mouseHeldDownOnScrollbar;

	public bool MouseHeldDownOnScrollbar
	{
		set
		{
			mouseHeldDownOnScrollbar = value;

			if (!mouseHeldDownOnScrollbar && !hovering)
				AnimateScrollbar(false);
		}
	}

	/// <summary>
	/// Animates the scrollbar into visibility
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		AnimateScrollbar(true);
		hovering = true;
	}

	/// <summary>
	/// Animates the scrollbar out of visibility
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!mouseHeldDownOnScrollbar)
			AnimateScrollbar(false);

		hovering = false;
	}

	/// <summary>
	/// Animates the scrollbar graphic
	/// </summary>
	/// <param name="isHovered"> If the mouse is hovered over the list or not </param>
	private void AnimateScrollbar(bool isHovered)
	{
		scrollBar.AnimateGraphic(isHovered ? 1f : 0f, 0.15f);
		scrollbarHandle.AnimateGraphic(isHovered ? 1f : 0f, 0.15f);
	}
}
