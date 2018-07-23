using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public abstract class InfoIconBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	protected PopupManager popupManager;

	[Inject]
	public void Construct(PopupManager popupManager)
	{
		this.popupManager = popupManager;
	}

	public abstract void OnPointerEnter(PointerEventData eventData);

	public abstract void OnPointerExit(PointerEventData eventData);
}