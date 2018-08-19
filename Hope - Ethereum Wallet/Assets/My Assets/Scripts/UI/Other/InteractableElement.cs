using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private bool isButton, exitableButton;

	private bool hovering;

	private Texture2D textCursor, handCursor;

	private Button buttonComponent;

	/// <summary>
	/// Loads the text cursor from the resources folder
	/// </summary>
	void Awake()
	{
		if (isButton)
		{
			buttonComponent = transform.GetComponent<Button>();

			if (exitableButton)
				buttonComponent.onClick.AddListener(() => SetCursor(false));
		}

		textCursor = Resources.Load("UI/Graphics/Textures/New/Icons/TextCursor_Icon") as Texture2D;
		handCursor = Resources.Load("UI/Graphics/Textures/New/Icons/HandCursor_Icon") as Texture2D;
	}

	private void Update()
	{
		if (hovering && isButton)
		{
			SetCursor(buttonComponent.interactable);
		}
	}

	/// <summary>
	/// Sets cursor image to the text cursor icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((isButton && buttonComponent.interactable) || !isButton)
			SetCursor(true);

		hovering = true;
	}

	/// <summary>
	/// Sets cursor image back to the default
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData)
	{
		SetCursor(false);
		hovering = false;
	}

	/// <summary>
	/// Sets the cursor image either to the hand cursor, text cursor, or default cursor
	/// </summary>
	/// <param name="customCursor"> Whether the cursor needs to be changed to a customCursor, or the defualt cursor </param>
	private void SetCursor(bool customCursor)
	{
		Cursor.SetCursor(customCursor ? (isButton ? handCursor : textCursor) : null, new Vector2(isButton ? 12f : 60f, isButton ? 5f : 25f), CursorMode.Auto);
	}
}
