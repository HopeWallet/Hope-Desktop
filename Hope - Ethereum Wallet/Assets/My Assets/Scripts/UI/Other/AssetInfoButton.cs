using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssetInfoButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Transform myTransform;
	private Button button;

	private bool clicked;

	/// <summary>
	/// Sets the variables
	/// </summary>
	private void Awake()
	{
		myTransform = transform;
		button = GetComponent<Button>();

		button.onClick.AddListener(() => clicked = true);
	}

	/// <summary>
	/// Resets the button.
	/// </summary>
	public void ResetButton()
	{
		clicked = false;
		myTransform.localScale /= 1.1f;
	}

	/// <summary>
	/// Pointer hovers over the text button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!button.interactable)
			return;

		myTransform.localScale *= 1.1f;
	}

	/// <summary>
	/// Pointer leaves the text button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!button.interactable)
			return;

		if (!clicked)
			myTransform.localScale /= 1.1f;
	}
}
