﻿using UnityEngine;
using UnityEngine.UI;

public class InteractableInputField : InteractableBase
{
	private Selectable inputField;

	/// <summary>
	/// Sets the appropriate values
	/// </summary>
	private void Start()
	{
		inputField = transform.GetComponent<Selectable>();

		customCursor = Resources.Load("UI/Graphics/Textures/Icons/TextCursor_Icon") as Texture2D;
		customCursorPosition = new Vector2(60f, 45f);
	}

	public override void OnCustomPointerEnter()
	{
		if (inputField.interactable)
			SetCursor(customCursor);
	}
}
