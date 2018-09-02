using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoreDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Button moreButton;
	private GameObject clickedImage;

	[SerializeField] private GameObject moreDropdown;
	[SerializeField] private GameObject box;
	[SerializeField] private GameObject triangle;
	[SerializeField] private Button[] subButtons;

	private bool dropdownOpen, hovering;

	/// <summary>
	/// Sets the button listeners
	/// </summary>
	private void Awake()
	{
		moreButton = transform.GetComponent<Button>();
		clickedImage = transform.GetChild(0).gameObject;
		moreButton.onClick.AddListener(MoreButtonClicked);
		subButtons.ForEach(button => button.onClick.AddListener(MoreButtonClicked));
	}

	/// <summary>
	/// Disables the dropdown if user clicks anywhere else on the screen
	/// </summary>
	private void Update()
	{
		if (dropdownOpen && !hovering)
		{
			if (Input.GetMouseButtonDown(0))
				MoreButtonClicked();
		}
	}

	/// <summary>
	/// Pointer enters the More button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerEnter(PointerEventData eventData) => hovering = true;

	/// <summary>
	/// Pointer exits the More button
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData) => hovering = false;

	/// <summary>
	/// More button is clicked
	/// </summary>
	private void MoreButtonClicked()
	{
		clickedImage.SetActive(!clickedImage.activeInHierarchy);
		moreButton.transition = clickedImage.activeInHierarchy ? Selectable.Transition.None : Selectable.Transition.SpriteSwap;
		dropdownOpen = !dropdownOpen;

		if (clickedImage.activeInHierarchy)
			AnimateDropdownIn();
		else
			AnimateDropdownOut();
	}

	/// <summary>
	/// Animates the dropdown into view
	/// </summary>
	private void AnimateDropdownIn()
	{
		moreDropdown.AnimateScale(1f, 0.1f);
		box.AnimateGraphic(1f, 0.1f);
		triangle.AnimateGraphic(1f, 0.1f);

		float timeValue = 0.15f;

		foreach (Button button in subButtons)
		{
			button.transform.GetChild(0).gameObject.AnimateGraphicAndScale(1f, 1f, timeValue, () => button.interactable = true);
			timeValue += 0.05f;
		}
	}

	/// <summary>
	/// Animates the dropdown out of view
	/// </summary>
	private void AnimateDropdownOut()
	{
		box.AnimateGraphic(0f, 0.1f, () => moreDropdown.transform.localScale = Vector2.zero);
		triangle.AnimateGraphic(0f, 0.1f);

		foreach (Button button in subButtons)
		{
			button.interactable = false;
			button.gameObject.transform.GetChild(0).gameObject.AnimateGraphicAndScale(0f, 0f, 0.1f);
		}
	}
}
