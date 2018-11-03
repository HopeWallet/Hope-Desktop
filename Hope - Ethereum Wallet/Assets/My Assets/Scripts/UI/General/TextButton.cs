using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class that manages the visuals of a text button when a user interacts with it
/// </summary>
public sealed class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	private TextMeshProUGUI text;
	private Button button;

	private bool mouseHeldDown, clicked;

	/// <summary>
	/// Sets the variables
	/// </summary>
	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
		button = GetComponent<Button>();
	}

    /// <summary>
    /// Resets the button.
    /// </summary>
    public void ResetButton()
    {
        clicked = false;
        text.fontSize /= 1.1f;
        text.fontStyle = FontStyles.Normal;
    }

	/// <summary>
	/// Pointer hovers over the text button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!button.interactable)
			return;

		text.fontSize *= 1.1f;

		if (mouseHeldDown)
			text.fontStyle = FontStyles.Underline;
	}

	/// <summary>
	/// Pointer leaves the text button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!button.interactable)
			return;

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
		if (!button.interactable)
			return;

		mouseHeldDown = true;
		text.fontStyle = FontStyles.Underline;
	}

	/// <summary>
	/// Pointer click is released
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerUp(PointerEventData eventData)
	{
		if (!button.interactable)
			return;

		mouseHeldDown = false;

		if (text.fontStyle == FontStyles.Underline)
			clicked = true;
	}
}
