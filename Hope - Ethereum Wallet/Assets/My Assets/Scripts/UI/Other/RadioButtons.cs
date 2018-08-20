using NBitcoin;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RadioButtons : MonoBehaviour
{
	public event Action<WordCount> OnButtonChanged;

	private int previouslySelectedButton;

	private void Awake()
	{
		for (int i = 0; i < transform.childCount; i++)
			SetButtonListener(i);
	}

	private void SetButtonListener(int index) => transform.GetChild(index).GetComponent<Button>().onClick.AddListener(() => RadioButtonClicked(index));

	public void RadioButtonClicked(int activeButton)
	{
		SetRadioButton(previouslySelectedButton, false);
		SetRadioButton(activeButton, true);

		previouslySelectedButton = activeButton;

		OnButtonChanged?.Invoke((WordCount)(12 + (activeButton * 3)));
	}

	private void SetRadioButton(int activeButton, bool active)
	{
		Transform ButtonTransform = transform.GetChild(activeButton);

		ButtonTransform.GetComponent<Button>().interactable = !active;
		ButtonTransform.GetChild(0).gameObject.AnimateColor(active ? UIColors.White : UIColors.Grey, 0.15f);
	}
}
