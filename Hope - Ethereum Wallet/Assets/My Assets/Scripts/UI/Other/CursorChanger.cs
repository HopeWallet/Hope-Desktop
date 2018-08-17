using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Texture2D textCursor;

	/// <summary>
	/// Loads the text cursor from the resources folder
	/// </summary>
	void Awake () => textCursor = Resources.Load("UI/Graphics/Textures/New/Icons/TextCursor_Icon") as Texture2D;

	/// <summary>
	/// Sets cursor image to the text cursor icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData) => Cursor.SetCursor(textCursor, new Vector2(45f, 25f), CursorMode.Auto);

	/// <summary>
	/// Sets cursor image back to the default
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData) => Cursor.SetCursor(null, new Vector2(45f, 25f), CursorMode.Auto);
}
