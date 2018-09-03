using UnityEngine;

public class InteractableInputField : InteractableBase
{
	/// <summary>
	/// Sets the appropriate values
	/// </summary>
	private void Start()
	{
		customCursor = Resources.Load("UI/Graphics/Textures/Icons/TextCursor_Icon") as Texture2D;
		customCursorPosition = new Vector2(60f, 25f);
	}
}
