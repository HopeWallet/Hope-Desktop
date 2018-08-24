using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollbarClickManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private ListScrollbarManager listScrollbarManager;

	public void OnPointerDown(PointerEventData eventData) => listScrollbarManager.MouseHeldDownOnScrollbar = true;

	public void OnPointerUp(PointerEventData eventData) => listScrollbarManager.MouseHeldDownOnScrollbar = false;
}
