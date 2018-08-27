using NBitcoin;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RadioButtons : MonoBehaviour
{
	public event Action<WordCount> OnButtonChanged;

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

		OnButtonChanged?.Invoke((WordCount)(12 + (activeButton * 3)));
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
		ButtonTransform.GetChild(0).gameObject.AnimateColor(active ? UIColors.White : UIColors.LightGrey, 0.15f);
	}
}
