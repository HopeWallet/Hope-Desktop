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
	private bool hovered;

	public PopupManager PopupManager { get; set; }

	public static event Action AnimatePopupOut;

	private void Awake() => hoverableIcon = iconType == IconType.Info || iconType == IconType.Error;

	/// <summary>
	/// Mouse entered the icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		AnimateIcon(1.3f);
		if (!hoverableIcon) return;
		hovered = true;
		StartCoroutine(WaitTime());
	}

	/// <summary>
	/// Mouse exited the icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData = null)
	{
		hovered = false;
		AnimateIcon(1f);
		if (!hoverableIcon) return;
		AnimatePopupOut?.Invoke();
	}

	/// <summary>
	/// Enables the icon to a scale value of 0f
	/// </summary>
	public void EnableIcon() => AnimateIcon(1f);

	/// <summary>
	/// Disables the icon to a scale value of 0f
	/// </summary>
	public void DisableIcon()
	{
		Action onCompleteAction = null;

		if (hovered)
		{
			hovered = false;
			AnimatePopupOut?.Invoke();
			hoverableIcon = false;
			onCompleteAction = () => hovered = false;
		}

		AnimateIcon(0f);
	}

	/// <summary>
	/// Animates the icon
	/// </summary>
	/// <param name="value"> The end value of the graphic and scale being animated to </param>
	/// <param name="onCompleteAction"></param>
	public void AnimateIcon(float value, Action onCompleteAction = null)
	{
		if (hovered && value != 0f) return;

		gameObject.AnimateGraphicAndScale(value, value, 0.1f, () => onCompleteAction?.Invoke());
	}

	private IEnumerator WaitTime()
	{
		yield return new WaitForSeconds(0.15f);

		if (hovered)
			PopupManager.GetPopup<InfoPopup>(true).SetUIElements(infoTitle, infoText, iconType, transform.position);
	}

	public enum IconType { Info, Error, CheckMark }
}