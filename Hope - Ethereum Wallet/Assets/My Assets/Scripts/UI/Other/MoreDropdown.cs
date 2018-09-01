using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoreDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Button moreButton;

	[SerializeField] private GameObject moreDropdown;
	[SerializeField] private GameObject box;
	[SerializeField] private GameObject triangle;
	[SerializeField] private Button[] subButtons;

	private bool dropdownOpen, hovering;

	private void Awake()
	{
		moreButton = transform.GetComponent<Button>();
		moreButton.onClick.AddListener(() => { moreButton.interactable = false; AnimateDropdownIn(); });
		subButtons.ForEach(button => button.onClick.AddListener(AnimateDropdownOut));
	}

	private void Update()
	{
		if (dropdownOpen && !hovering)
		{
			if (Input.GetMouseButtonDown(0))
				AnimateDropdownOut();
		}
	}

	public void OnPointerEnter(PointerEventData eventData) => hovering = true;

	public void OnPointerExit(PointerEventData eventData) => hovering = false;

	private void AnimateDropdownIn()
	{
		dropdownOpen = true;

		moreDropdown.AnimateScale(1f, 0.1f);
		box.AnimateGraphic(1f, 0.1f);
		triangle.AnimateGraphic(1f, 0.1f);

		float timeValue = 0.125f;

		foreach (Button button in subButtons)
		{
			button.transform.GetChild(0).gameObject.AnimateGraphicAndScale(1f, 1f, timeValue, () => button.interactable = true);
			timeValue += 0.025f;
		}
	}

	private void AnimateDropdownOut()
	{
		dropdownOpen = false;

		box.AnimateGraphic(0f, 0.1f, () => { moreButton.interactable = true; moreDropdown.transform.localScale = Vector2.zero; });
		triangle.AnimateGraphic(0f, 0.1f);

		foreach (Button button in subButtons)
		{
			button.interactable = false;
			button.gameObject.transform.GetChild(0).gameObject.AnimateGraphicAndScale(1f, 1f, 0.1f);
		}
	}
}
