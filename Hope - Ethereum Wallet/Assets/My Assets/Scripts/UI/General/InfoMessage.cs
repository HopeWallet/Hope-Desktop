using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine;

public sealed class InfoMessage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	public PopupManager PopupManager { get; set; }

	public string titleText;
	public string bodyText;
	public bool infoIcon;

	/// <summary>
	/// Mouse entered the icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		PopupManager.GetPopup<InfoPopup>(true).SetUIElements(titleText, bodyText, infoIcon, transform.localPosition);
		AnimateIconScale(true);
	}

	/// <summary>
	/// Mouse exited the icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData)
	{
		PopupManager.CloseActivePopup();
		AnimateIconScale(false);
	}

	/// <summary>
	/// Animates the scale X and Y of the icon
	/// </summary>
	/// <param name="animateIn"> Checks if animating it bigger, or smaller </param>
	private void AnimateIconScale(bool animateBigger)
	{
		transform.DOScaleX(animateBigger ? 1.3f : 1f, 0.1f);
		transform.DOScaleY(animateBigger ? 1.3f : 1f, 0.1f);
	}
}