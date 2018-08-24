using UnityEngine;
using UnityEngine.EventSystems;

public class ListScrollbarManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	[SerializeField] private GameObject scrollBar, scrollbarHandle;

	public void OnPointerEnter(PointerEventData eventData) => AnimateScrollbar(1f);

	public void OnPointerExit(PointerEventData eventData) => AnimateScrollbar(0f);

	private void AnimateScrollbar(float value)
	{
		scrollBar.AnimateGraphic(value, 0.15f);
		scrollbarHandle.AnimateGraphic(value, 0.15f);
	}
}
