﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the More button dropdown
/// </summary>
public sealed class MoreDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public static Action PopupClosed { get; private set; }

	private Button moreButton;
	private GameObject clickedImage;

	[SerializeField] private GameObject moreDropdown;
	[SerializeField] private GameObject box;
	[SerializeField] private GameObject triangle;
	[SerializeField] private Button[] subButtons;

	private PopupManager popupManager;

	private bool dropdownOpen, hovering, popupIsOpen;

	/// <summary>
	/// Sets the popupManager
	/// </summary>
	/// <param name="popupManager"> The active PopupManager </param>
	[Inject]
	public void Construct(PopupManager popupManager) => this.popupManager = popupManager;

	/// <summary>
	/// Sets the button listeners
	/// </summary>
	private void Awake()
	{
		moreButton = transform.GetComponent<Button>();
		clickedImage = transform.GetChild(0).gameObject;
		moreButton.onClick.AddListener(MoreButtonClicked);

		for (int i = 0; i < subButtons.Length; i++)
			SetButtonListener(i);
	}

	/// <summary>
	/// Disables the dropdown if user clicks anywhere else on the screen
	/// </summary>
	private void Update()
	{
		if (dropdownOpen && !hovering)
		{
			if (Input.GetMouseButtonDown(0) && !popupIsOpen)
				MoreButtonClicked();
		}
	}

	/// <summary>
	/// Sets the given button onClick listener
	/// </summary>
	/// <param name="num"> The number of the button in the array </param>
	private void SetButtonListener(int num) => subButtons[num].onClick.AddListener(() => DropdownButtonClicked(num));

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

		float duration = 0.15f;
		subButtons.ForEach(gameObject => { gameObject.transform.GetChild(0).gameObject.AnimateGraphicAndScale(1f, 1f, duration); duration += 0.05f; });
	}

	/// <summary>
	/// Animates the dropdown out of view
	/// </summary>
	private void AnimateDropdownOut()
	{
		box.AnimateGraphic(0f, 0.1f, () => moreDropdown.transform.localScale = Vector2.zero);
		triangle.AnimateGraphic(0f, 0.1f);

		foreach (Button button in subButtons)
			button.gameObject.transform.GetChild(0).gameObject.AnimateGraphicAndScale(0f, 0f, 0.1f);
	}

	/// <summary>
	/// A button in the dropdown has been clicked
	/// </summary>
	/// <param name="num"> The number of the button in the dropdown hiearchy </param>
	private void DropdownButtonClicked(int num)
	{
		popupIsOpen = true;
		subButtons[num].interactable = false;
		PopupClosed = () => { subButtons[num].interactable = true; popupIsOpen = false; };

		switch (num)
		{
			case 0:
				//Open up the help popup
				break;
			case 1:
				popupManager.GetPopup<AboutPopup>();
				break;
			case 2:
				popupManager.GetPopup<SettingsPopup>();
				break;
			case 3:
				popupManager.GetPopup<GeneralOkCancelPopup>().SetSubText("Are you sure you want to logout?").OnOkClicked(() => SceneManager.LoadScene("HopeWallet")).OnFinish(PopupClosed);
				break;
		}
	}
}