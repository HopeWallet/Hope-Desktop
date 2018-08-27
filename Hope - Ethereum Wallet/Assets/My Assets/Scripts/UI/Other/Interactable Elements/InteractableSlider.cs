using UnityEngine;

public class InteractableSlider : InteractableBase
{
	/// <summary>
	/// Sets the appropriate values
	/// </summary>
	private void Start()
	{
		customCursor = Resources.Load("UI/Graphics/Textures/New/Icons/HandCursor_Icon") as Texture2D;
		customCursorPosition = new Vector2(12f, 5f);
	}
}
