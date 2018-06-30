﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateWalletForm : FormAnimation
{

	[SerializeField] private GameObject form;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject walletNameField;
	[SerializeField] private GameObject passwordHeader;
	[SerializeField] private GameObject password1Field;
	[SerializeField] private GameObject password2Field;
	[SerializeField] private GameObject createButton;
	[SerializeField] private GameObject warningIcon;
	[SerializeField] private GameObject checkMarkIcon;

	private string walletNameText, password1Text, password2Text;
	private Button createButtonComponent;
	private TMP_InputField[] inputFields;

	protected override void InitializeElements()
	{
		InitializeVariable(ref walletNameField);
		InitializeVariable(ref password1Field);
		InitializeVariable(ref password2Field);

		createButtonComponent = createButton.GetComponent<Button>();
		createButtonComponent.interactable = false;

		inputFields = new TMP_InputField[3];
		inputFields[0] = walletNameField.GetComponent<TMP_InputField>();
		inputFields[1] = password1Field.GetComponent<TMP_InputField>();
		inputFields[2] = password2Field.GetComponent<TMP_InputField>();
	}

	private void Update()
	{
		for (int i = 0; i < 3; i++)
		{
			if (Input.GetKeyDown(KeyCode.Tab) && inputFields[i].isFocused)
			{
				if (i != 2)
					inputFields[++i].ActivateInputField();

				else
					inputFields[0].ActivateInputField();
			}
		}
	}

	#region Animating

	protected override void AnimateIn()
	{
		form.AnimateGraphicAndScale(1f, 1f, 0.2f,
			() => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
			() => createButton.AnimateGraphicAndScale(1f, 1f, 0.2f)));

		walletNameField.AnimateScaleX(1f, 0.15f,
			() => passwordHeader.AnimateScaleX(1f, 0.15f,
			() => password1Field.AnimateScaleX(1f, 0.15f,
			() => password2Field.AnimateScaleX(1f, 0.15f,
			() => FinishedAnimatingIn()))));

	}

	protected override void AnimateOut()
	{
		createButton.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => title.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => form.AnimateGraphicAndScale(0f, 0f, 0.15f,
			() => FinishedAnimatingOut())));

		walletNameField.AnimateScaleX(0f, 0.15f);
		passwordHeader.AnimateScaleX(0f, 0.15f);
		password1Field.AnimateScaleX(0f, 0.15f);
		password2Field.AnimateScaleX(0f, 0.15f);

		warningIcon.AnimateGraphic(0f, 0.1f);
		checkMarkIcon.AnimateGraphic(0f, 0.1f);
	}

	#endregion

	#region Other Methods

	/// <summary>
	/// Adds listeners to a given input field
	/// </summary>
	/// <param name="gameObject">The gameobject input field</param>
	private void InitializeVariable(ref GameObject gameObject)
	{
		var inputField = gameObject.GetComponent<TMP_InputField>();
		inputField.onValueChanged.AddListener(str => SetText());
		inputField.onValueChanged.AddListener(str => SetButtonInteractable());
	}

	/// <summary>
	/// Sets the string variables to the current component's text
	/// </summary>
	private void SetText()
	{
		walletNameText = walletNameField.GetComponent<TMP_InputField>().text;
		password1Text = password1Field.GetComponent<TMP_InputField>().text;
		password2Text = password2Field.GetComponent<TMP_InputField>().text;
	}

	/// <summary>
	/// Checks if passwords match, are above 7 characters, and all fieldss are filled in
	/// </summary>
	private void SetButtonInteractable()
	{
		bool passwordsValid = password1Text == password2Text && password1Text.Length >= 8;

		if (password1Text != "" && password2Text != "")
		{
			warningIcon.AnimateGraphic(passwordsValid ? 0f : 1f, 0.25f);
			checkMarkIcon.AnimateGraphic(passwordsValid ? 1f : 0f, 0.25f);
		}

		else
		{
			warningIcon.AnimateGraphic(0f, 0.25f);
			checkMarkIcon.AnimateGraphic(0f, 0.25f);
		}

		createButtonComponent.interactable = walletNameText != "" && passwordsValid;
	}

	#endregion

}
