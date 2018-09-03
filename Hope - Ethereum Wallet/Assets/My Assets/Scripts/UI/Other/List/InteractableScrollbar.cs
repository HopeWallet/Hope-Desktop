using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableScrollbar : InteractableBase, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private ListScrollbarManager listScrollbarManager;

	private bool mouseHeldDown;

	public bool MouseHeldDown
	{
		get { return mouseHeldDown; }
		set
		{
			mouseHeldDown = value;

			if (!mouseHeldDown && !listScrollbarManager.Hovering)
			{
				listScrollbarManager.AnimateScrollbar(false);
				SetCursor(null);
			}
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
	/// The mouse is being held down
	/// </summary>
	/// <param name="eventData"> The PointerEventData</param>
	public void OnPointerDown(PointerEventData eventData) => MouseHeldDown = true;

	/// <summary>
	/// The mouse click is released
	/// </summary>
	/// <param name="eventData"> The PointerEventData</param>
	public void OnPointerUp(PointerEventData eventData) => MouseHeldDown = false;

	/// <summary>
	/// Sets the cursor to the custom cursor image
	/// </summary>
	public override void OnCustomPointerEnter() => SetCursor(customCursor);

	/// <summary>
	/// Sets the cursor to the default image
	/// </summary>
	public override void OnCustomPointerExit()
	{
		if (!mouseHeldDown)
			SetCursor(null);
	}
}
