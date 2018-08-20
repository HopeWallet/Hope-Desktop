using System;
using UnityEngine;
using UnityEngine.UI;

public class RadioButtons : MonoBehaviour
{
	public event Action<WordCount> OnButtonChanged;

	private int previouslySelectedButton;

	private readonly Color INACTIVE_COLOR = new Color(0.65f, 0.65f, 0.65f);
	private readonly Color ACTIVE_COLOR = new Color(0.85f, 0.85f, 0.85f);

	public void RadioButtonClicked(int activeButton)
	{
		SetRadioButton(previouslySelectedButton, false);
		SetRadioButton(activeButton, true);

		OnButtonChanged?.Invoke((WordCount)activeButton);

		previouslySelectedButton = activeButton;
	}

	private void SetRadioButton(int activeButton, bool active)
	{
		Transform ButtonTransform = transform.GetChild(activeButton);

		ButtonTransform.GetComponent<Button>().interactable = !active;
		ButtonTransform.GetChild(0).gameObject.AnimateColor(active ? ACTIVE_COLOR : INACTIVE_COLOR, 0.1f);
	}

	public enum WordCount { TwelveWords, TwentyFourWords }
}
