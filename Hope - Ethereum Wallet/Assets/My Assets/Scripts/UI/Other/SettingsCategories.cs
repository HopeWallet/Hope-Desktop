using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCategories : MonoBehaviour {

	public event Action<Category> OnCategoryChanged;

	private int previouslySelectedButton;

	/// <summary>
	/// Sets the variables of the radio buttons
	/// </summary>
	private void Awake()
	{
		for (int i = 0; i < transform.childCount; i++)
			SetButtonListener(i);
	}

	/// <summary>
	/// Sets the button listener for the given index
	/// </summary>
	/// <param name="index"> The index of the button in the hiearchy </param>
	private void SetButtonListener(int index) => transform.GetChild(index).GetComponent<Button>().onClick.AddListener(() => RadioButtonClicked(index));

	public void RadioButtonClicked(int activeButton)
	{
		SetRadioButton(previouslySelectedButton, false);
		SetRadioButton(activeButton, true);

		previouslySelectedButton = activeButton;

		OnCategoryChanged?.Invoke((Category)activeButton);
	}

	/// <summary>
	/// Changes the visuals of the newly active, and previously active radio button
	/// </summary>
	/// <param name="activeButton"> the index of the button being changed </param>
	/// <param name="active"> Whether the button is currently active or not </param>
	private void SetRadioButton(int activeButton, bool active)
	{
		Transform ButtonTransform = transform.GetChild(activeButton);

		ButtonTransform.GetComponent<Button>().interactable = !active;
		ButtonTransform.GetComponent<TextMeshProUGUI>().color = active ? UIColors.Green : UIColors.White;
		ButtonTransform.GetChild(0).gameObject.AnimateGraphic(active ? 1f : 0f, 0.15f);
	}

	public enum Category { General, WalletAndAddress }
}
