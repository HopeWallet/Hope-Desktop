using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBoxAnimator : MonoBehaviour
{

	private Button checkBoxButton;
	private GameObject checkMarkIcon;

	private bool toggledOn;

	public bool ToggledOn
	{
		get { return toggledOn; }
		set { toggledOn = value; }
	}

	private void Awake()
	{
		checkBoxButton = transform.GetComponent<Button>();
		checkBoxButton.onClick.AddListener(CheckBoxClicked);
		checkMarkIcon = transform.GetChild(0).gameObject;
		toggledOn = checkMarkIcon.transform.localScale.x > 0 ? true : false;
	}

	private void CheckBoxClicked()
	{
		checkMarkIcon.AnimateGraphicAndScale(toggledOn ? 0f : 1f, toggledOn ? 0f : 1f, 0.15f);
		toggledOn = !toggledOn;
	}
}
