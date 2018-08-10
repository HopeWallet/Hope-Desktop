using UnityEngine.EventSystems;
using UnityEngine;
using System;
using System.Collections;

public sealed class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public string infoTitle;
	public string infoText;
	public IconType iconType;

	private bool hoverableIcon;
	private bool hovering;

	public PopupManager PopupManager { get; set; }

	public static event Action AnimatePopupOut;

	private void Awake() => hoverableIcon = iconType == IconType.Info || iconType == IconType.Error;

	private void Update()
	{
		if (!hovering)
			return;

		if (PopupManager.ActivePopupType != typeof(InfoPopup))
		{
			hovering = false;
		}
	}

	/// <summary>
	/// Mouse entered the icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!hoverableIcon)
			return;

		if (PopupManager.ActivePopupType == typeof(InfoPopup))
		{
			hovering = true;
			return;
		}

		PopupManager.GetPopup<InfoPopup>(true).SetUIElements(infoTitle, infoText, iconType, transform.position);
		hovering = true;
	}

	/// <summary>
	/// Mouse exited the icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData = null)
	{
		if (!hoverableIcon)
			return;

		AnimatePopupOut?.Invoke();
	}

	/// <summary>
	/// Animates the icon
	/// </summary>
	/// <param name="value"> The end value of the graphic and scale being animated to </param>
	/// <param name="onCompleteAction"></param>
	public void AnimateIcon(float value)
	{
		if (hovering)
			return;

		gameObject.AnimateGraphicAndScale(value, value, 0.1f);
	}

	public enum IconType { Info, Error, CheckMark }
}