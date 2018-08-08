using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddTokenPopupAnimator : UIAnimator
{

	[SerializeField] private Image blur;
	[SerializeField] private GameObject dim;
	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject addressSection;
	[SerializeField] private GameObject symbolSection;
	[SerializeField] private GameObject decimalSection;
	[SerializeField] private GameObject tokenSection;
	[SerializeField] private GameObject addTokenButton;
	[SerializeField] private GameObject errorIcon;
	[SerializeField] private GameObject checkMarkIcon;

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
		
	}

	/// <summary>
	/// Animates the UI elements of the form into view
	/// </summary>
	protected override void AnimateIn()
	{

	}

	/// <summary>
	/// Animates the UI elements of the form out of view
	/// </summary>
	protected override void AnimateOut()
	{

	}
}
