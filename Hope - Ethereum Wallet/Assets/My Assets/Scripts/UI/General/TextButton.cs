using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public Action PopupClosed { get; private set; }

	private TextMeshProUGUI text;

	private bool clicked;

	/// <summary>
	/// Sets the variables
	/// </summary>
	private void Awake()
	{
		text = transform.GetComponent<TextMeshProUGUI>();
		PopupClosed = () => { clicked = false; text.fontSize /= 1.1f; };
	}

	/// <summary>
	/// pointer hovers over the text button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData) => text.fontSize *= 1.1f;

	/// <summary>
	/// User clicked on the button
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerClick(PointerEventData eventData) => clicked = true;

	/// <summary>
	/// pointer leaves the text button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!clicked)
			text.fontSize /= 1.1f;
	}
}
