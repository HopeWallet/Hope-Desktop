using UnityEngine;

public class InteractableGeneral : InteractableBase
{
	/// <summary>
	/// Sets the custom cursor and cursor position
	/// </summary>
	private void Start()
	{
		customCursor = Resources.Load("UI/Graphics/Textures/Icons/HandCursor_Icon") as Texture2D;
		customCursorPosition = new Vector2(12f, 5f);
	}
}
