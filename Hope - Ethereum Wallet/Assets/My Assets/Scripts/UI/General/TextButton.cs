using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class that manages the visuals of a text button when a user interacts with it
/// </summary>
public sealed class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public Action PopupClosed { get; private set; }

	private TextMeshProUGUI text;

	private bool mouseHeldDown, clicked;

	/// <summary>
	/// Sets the variables
	/// </summary>
	private void Awake()
	{
		text = transform.GetComponent<TextMeshProUGUI>();
		PopupClosed = () => { clicked = false; text.fontSize /= 1.1f; text.fontStyle = FontStyles.Normal; };
	}

	/// <summary>
	/// pointer hovers over the text button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		text.fontSize *= 1.1f;

		if (mouseHeldDown)
			text.fontStyle = FontStyles.Underline;
	}

	/// <summary>
	/// pointer leaves the text button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!clicked)
		{
			text.fontSize /= 1.1f;
			text.fontStyle = FontStyles.Normal;
		}
	}

	/// <summary>
	/// Pointer is held down on the text button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerDown(PointerEventData eventData)
	{
		mouseHeldDown = true;
		text.fontStyle = FontStyles.Underline;
	}

	/// <summary>
	/// Pointer click is released
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerUp(PointerEventData eventData)
	{
		mouseHeldDown = false;

		if (text.fontStyle == FontStyles.Underline)
			clicked = true;
	}
}
