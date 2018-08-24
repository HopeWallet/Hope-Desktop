using UnityEngine;
using UnityEngine.UI;

public class InteractableButton : InteractableBase
{
	[SerializeField] private bool exitableButton;

	private Button buttonComponent;

	private bool hovering;

	/// <summary>
	/// Sets the appropriate values
	/// </summary>
	private void Start()
	{
		customCursor = Resources.Load("UI/Graphics/Textures/New/Icons/HandCursor_Icon") as Texture2D;
		customCursorPosition = new Vector2(12f, 5f);

		buttonComponent = transform.GetComponent<Button>();

		if (exitableButton)
			buttonComponent.onClick.AddListener(OnCustomPointerExit);
	}

	/// <summary>
	/// If the user is hovering over the button, it sets the cursor depending on if the button is interactable or not
	/// </summary>
	private void Update()
	{
		if (hovering)
			SetCursor(buttonComponent.interactable);
	}

	/// <summary>
	/// Sets the custom cursor and changes the hovering variable
	/// </summary>
	public override void OnCustomPointerEnter()
	{
		SetCursor(buttonComponent.interactable);
		hovering = true;
	}

	/// <summary>
	/// Sets the custom cursor and changes the hovering variable
	/// </summary>
	public override void OnCustomPointerExit()
	{
		SetCursor(false);
		hovering = false;
	}
}
