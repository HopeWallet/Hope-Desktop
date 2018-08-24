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

	private void Start()
	{
		customCursor = Resources.Load("UI/Graphics/Textures/New/Icons/HandCursor_Icon") as Texture2D;
		customCursorPosition = new Vector2(12f, 5f);
	}

	private void Update()
	{
		if (mouseHeldDown)
			SetCursor(customCursor);
	}

	public void OnPointerDown(PointerEventData eventData) => MouseHeldDown = true;

	public void OnPointerUp(PointerEventData eventData) => MouseHeldDown = false;

	public override void OnCustomPointerEnter() => SetCursor(customCursor);

	public override void OnCustomPointerExit()
	{
		if (!mouseHeldDown)
			SetCursor(null);
	}
}
