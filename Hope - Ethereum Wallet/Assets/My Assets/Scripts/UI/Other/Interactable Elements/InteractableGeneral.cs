using UnityEngine;

public class InteractableGeneral : InteractableBase
{
	private void Start()
	{
		customCursor = Resources.Load("UI/Graphics/Textures/Icons/HandCursor_Icon") as Texture2D;
		customCursorPosition = new Vector2(12f, 5f);
	}
}
