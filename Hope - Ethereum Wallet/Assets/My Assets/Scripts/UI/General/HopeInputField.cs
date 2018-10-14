using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HopeInputField : MonoBehaviour
{
	public event Action<string> OnInputUpdated;

	[SerializeField] private InputField inputFieldBase;
	[SerializeField] private GameObject placeholder;
	[SerializeField] private GameObject eye;
	[SerializeField] private GameObject errorIcon;
	[SerializeField] private bool noSpaces;
	[SerializeField] private bool placeholderFadeAway;

	public TextMeshProUGUI errorMessage;

	private Sprite eyeInactiveNormal;
	private Sprite eyeActiveNormal;

	private string text;

	public InputField InputFieldBase => inputFieldBase;

	private string characterPlaceholders = "頁設是煵엌嫠쯦案煪㍱從つ浳浤搰㍭煤洳橱橱迎事網計簡大㍵畱煵田煱둻睤㌹楤ぱ椹ぱ頹衙";

	public string Text
	{
		get { return text; }
		set
		{
			text = value;
			inputFieldBase.text = text;
		}
	}

	public List<byte> Bytes = new List<byte>();

	public bool Error { get; set; }

	private bool assigningRandomCharacter;

	/// <summary>
	/// Sets the variables and inputfield listener
	/// </summary>
	private void Awake()
	{
		inputFieldBase.onValueChanged.AddListener(InputFieldChanged);

		if (eye != null)
		{
			eye.GetComponent<Button>().onClick.AddListener(EyeClicked);

			SetSprite(ref eyeInactiveNormal, "Eye_Inactive");
			SetSprite(ref eyeActiveNormal, "Eye_Active");
		}

		Error = true;
		Text = string.Empty;

		//byte[] chineseBytes = characterPlaceholders.GetUTF8Bytes();
		//chineseBytes.Log();
		//chineseBytes.GetUTF8String().Log();
		//chineseBytes.Length.Log();
		//characterPlaceholders.Length.Log();
	}

	public override bool Equals(object hopeInputField)
	{
		if (hopeInputField.GetType() != typeof(HopeInputField))
			return false;

		HopeInputField inputField = hopeInputField as HopeInputField;

		if (inputFieldBase.inputType == InputField.InputType.Password)
		{
			if (Bytes.Count != inputField.Bytes.Count)
				return false;

			for (int i = 0; i < Bytes.Count; i++)
			{
				if (Bytes[i] != inputField.Bytes[i])
					return false;
			}
		}
		else
		{
			if (text != inputField.Text)
				return false;
		}

		return true;
	}

	/// <summary>
	/// Sets the target sprite from a given string
	/// </summary>
	/// <param name="targetSprite"> The target sprite being set </param>
	/// <param name="iconName"> The name of the icon file </param>
	private void SetSprite(ref Sprite targetSprite, string iconName)
	{
		Texture2D loadedTexture = Resources.Load("UI/Graphics/Textures/Icons/" + iconName) as Texture2D;
		targetSprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
	}

	/// <summary>
	/// Updates the visuals of the input field
	/// </summary>
	/// <param name="emptyString"> Whether the string is empty or not </param>
	public void UpdateVisuals()
	{
		bool emptyString = string.IsNullOrEmpty(inputFieldBase.text);

		inputFieldBase.gameObject.AnimateColor(emptyString ? UIColors.White : Error ? UIColors.Red : UIColors.Green, 0.15f);

		if (placeholder != null)
		{
			if (placeholderFadeAway)
				placeholder.AnimateColor(emptyString ? UIColors.LightGrey : new Color(1f, 1f, 1f, 0f), 0.15f);
			else
				placeholder.AnimateTransformY(emptyString ? 0f : 35f, 0.15f);
		}

		if (errorIcon != null) errorIcon.AnimateGraphic(emptyString ? 0f : Error ? 1f : 0f, 0.15f);

		if (errorMessage != null) errorMessage.gameObject.AnimateGraphic(emptyString ? 0f : Error ? 1f : 0f, 0.15f);

		if (eye != null)
			eye.AnimateGraphicAndScale(emptyString ? 0f : 1f, emptyString ? 0f : 1f, 0.15f);
	}

	/// <summary>
	/// The input field is changed
	/// </summary>
	/// <param name="inputString"> The text in the input field s</param>
	private void InputFieldChanged(string inputString)
	{
		if (inputFieldBase.inputType == InputField.InputType.Password)
			HidePasswordText();
		else
			Text = noSpaces ? inputString.Trim() : inputString;

		OnInputUpdated?.Invoke(inputString);

		UpdateVisuals();
	}

	private void HidePasswordText()
	{
		if (assigningRandomCharacter)
		{
			assigningRandomCharacter = false;
			return;
		}

		SetByteList();

		string tempString = string.Empty;

		for (int i = 0; i < inputFieldBase.text.Length; i++)
			tempString += characterPlaceholders[i];

		assigningRandomCharacter = true;

		inputFieldBase.text = tempString;
	}

	private void SetByteList()
	{
		if (Bytes.Count == 0)
		{
			Bytes.Add(inputFieldBase.text.GetUTF8Bytes().Single());
		}
		else if (inputFieldBase.text.Length == 0)
		{
			Bytes = new List<byte>();
		}
		else
		{
			List<byte> newByteList = new List<byte>();

			for (int i = 0; i < inputFieldBase.text.Length; i++)
			{
				if (inputFieldBase.text[i] != characterPlaceholders[i])
					newByteList.Add(inputFieldBase.text[i].ToString().GetUTF8Bytes().Single());
				else if (i == Bytes.Count)
					newByteList.Add(characterPlaceholders[i].ToString().GetUTF8Bytes().Single());
				else
					newByteList.Add(Bytes[i]);
			}

			Bytes = newByteList;
		}

		Bytes.GetUTF8String().Log();
		Bytes.ToArray().LogArray();
	}

	/// <summary>
	/// The eye icon is clicked and either enables or disables vision of the password
	/// </summary>
	private void EyeClicked()
	{
		if (inputFieldBase.contentType == InputField.ContentType.Password)
		{
			inputFieldBase.contentType = InputField.ContentType.Standard;
			inputFieldBase.text = Bytes.GetUTF8String();
		}
		else
		{
			inputFieldBase.contentType = InputField.ContentType.Password;
			HidePasswordText();
			inputFieldBase.text.ForEach(_ => inputFieldBase.textComponent.text += "*");
		}

		eye.GetComponent<Image>().sprite = inputFieldBase.contentType == InputField.ContentType.Password ? eyeInactiveNormal : eyeActiveNormal;
	}

	/// <summary>
	/// Sets the placeholder text for the input field
	/// </summary>
	/// <param name="placeholderText"> The text to set it to </param>
	public void SetPlaceholderText(string placeholderText) => placeholder.GetComponent<TextMeshProUGUI>().text = placeholderText;
}
