using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableSlider : InteractableBase, IPointerDownHandler, IPointerUpHandler
{
	private bool hovering, mouseHeldDown;

	public bool MouseHeldDown
	{
		get { return mouseHeldDown; }
		set
		{
			mouseHeldDown = value;

			if (!mouseHeldDown && !hovering)
				SetCursor(null);
		}
	}

	/// <summary>
	/// Sets the appropriate values
	/// </summary>
	private void Start()
	{
		customCursor = Resources.Load("UI/Graphics/Textures/Icons/HandCursor_Icon") as Texture2D;
		customCursorPosition = new Vector2(12f, 5f);
	}


	/// <summary>
	/// Makes sure that if the mouse is being held down, the right cursor is being shown
	/// </summary>
	private void Update()
	{
		if (mouseHeldDown)
			SetCursor(customCursor);
	}

	/// <summary>
	/// Sets the custom cursor and changes the hovering variable
	/// </summary>
	public override void OnCustomPointerEnter()
	{
		SetCursor(customCursor);
		hovering = true;
	}

	/// <summary>
	/// Sets the cursor to the default image if the mouse is not being held down and changes the hovering variable
	/// </summary>
	public override void OnCustomPointerExit()
	{
		if (!mouseHeldDown)
			SetCursor(null);

		hovering = false;
	}

	/// <summary>
	/// The mouse is being held down
	/// </summary>
	/// <param name="eventData"> The PointerEventData</param>
	public void OnPointerDown(PointerEventData eventData) => MouseHeldDown = true;

	/// <summary>
	/// The mouse click is released
	/// </summary>
	/// <param name="eventData"> The PointerEventData</param>
	public void OnPointerUp(PointerEventData eventData) => MouseHeldDown = false;
}
