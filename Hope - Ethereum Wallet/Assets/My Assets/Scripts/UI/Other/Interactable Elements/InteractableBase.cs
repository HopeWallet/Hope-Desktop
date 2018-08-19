using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	protected Texture2D customCursor;
	protected Vector2 customCursorPosition;

	/// <summary>
	/// Called when the mouse exits the object's raycast area
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData) => OnCustomPointerEnter();

	/// <summary>
	/// Called when the mouse enters the object's raycast area
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData) => OnCustomPointerExit();

	/// <summary>
	/// The custom pointer enter
	/// </summary>
	public virtual void OnCustomPointerEnter() => SetCursor(true);

	/// <summary>
	/// The custom pointer exit
	/// </summary>
	public virtual void OnCustomPointerExit() => SetCursor(false);

	/// <summary>
	/// Sets the cursor image either to the hand cursor, text cursor, or default cursor
	/// </summary>
	/// <param name="customCursor"> Whether the cursor needs to be changed to a customCursor, or the defualt cursor </param>
	protected void SetCursor(bool toCustomCursor) => Cursor.SetCursor(toCustomCursor ? customCursor : null, customCursorPosition, CursorMode.Auto);
}
