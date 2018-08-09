using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public sealed class InfoMessage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	public PopupManager PopupManager { get; set; }

	public string titleText;
	public string bodyText;
	public bool infoIcon;

	public bool Hovered { get; set; }

	public static event Action<bool> OnHoverChanged;

	/// <summary>
	/// Mouse entered the icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		Hovered = true;
		//OnHoverChanged?.Invoke(Hovered);
		PopupManager.GetPopup<InfoPopup>(true).SetUIElements(titleText, bodyText, infoIcon, transform.position);
		AnimateIconScale(true);
	}

	/// <summary>
	/// Mouse exited the icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData = null)
	{
		Hovered = false;
		OnHoverChanged?.Invoke(Hovered);
		AnimateIconScale(false);
	}

    /// <summary>
    /// Animates the scale X and Y of the icon
    /// </summary>
    /// <param name="animateBigger"> Checks if animating it bigger, or smaller </param>
    private void AnimateIconScale(bool animateBigger)
	{
		transform.DOScaleX(animateBigger ? 1.3f : 1f, 0.1f);
		transform.DOScaleY(animateBigger ? 1.3f : 1f, 0.1f);
	}
}