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

	private readonly string characterPlaceholders = "頁ｦء設ｧآ是ｨؤ煵ｩئ엌ｪب嫠ｫة쯦ｬض案ｭظ煪ｮغ㍱ｯف從ｱكつｲو浳ｳي浤ｴﺲ搰ﻼｵ㍭ｶﻨﺺ煤ｷﻫ洳ｸ橱ｹ橱ｺۻ迎ｻ事ﺨｼ網ｽ計ｾ簡ｿ大ﾀ㍵ﾁ畱ﾃ煵ﾄ田ﾅ煱ﾇ۾둻ﾈ睤ﾊ㌹ﾋ楤ﾌぱﾍ椹ﾎぱﾏ頹ﾐ衙";

	public string Text
	{
		get { return text; }
		set
		{
			text = value;
			inputFieldBase.text = text;
		}
	}

    public byte[] InputFieldBytes => Bytes.ToArray();

	public List<byte> Bytes = new List<byte>();

	public bool Error { get; set; }

	private bool assigningCharacterPlaceholders;

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
	}

	/// <summary>
	/// Checks if an input field's text is equal to another
	/// </summary>
	/// <param name="other"> The given object being checked </param>
	/// <returns></returns>
	public override bool Equals(object other)
	{
		if (other.GetType() != typeof(HopeInputField))
			return false;

		HopeInputField hopeInputField = other as HopeInputField;

		if (inputFieldBase.inputType == InputField.InputType.Password)
		{
			if (Bytes.Count != hopeInputField.Bytes.Count)
				return false;

            return Bytes.SequenceEqual(hopeInputField.Bytes);
		}
		else
		{
            return text == hopeInputField.Text;
		}
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


	/// <summary>
	/// Hides the password input field text with character placeholders, and adjusts teh bytes accordingly
	/// </summary>
	private void HidePasswordText()
	{
		if (assigningCharacterPlaceholders)
		{
			assigningCharacterPlaceholders = false;
			return;
		}

		SetByteList();

		string tempString = string.Empty;

		for (int i = 0; i < inputFieldBase.text.Length; i++)
			tempString += characterPlaceholders[i];

		assigningCharacterPlaceholders = true;

		inputFieldBase.text = tempString;
	}

	/// <summary>
	/// Changes the byte list to what the user has changed in the input field
	/// </summary>
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
			if (Bytes.Count > inputFieldBase.text.Length)
			{
				int numberOfCharactersRemoved = Bytes.Count - inputFieldBase.text.Length;
				int firstIndexRemoved = inputFieldBase.text.Length;

				for (int i = 0; i < inputFieldBase.text.Length; i++)
				{
					if (inputFieldBase.text[i] != characterPlaceholders[i])
					{
						firstIndexRemoved = i;
						break;
					}
				}

				for (int i = 0; i < numberOfCharactersRemoved; i++)
					Bytes.RemoveAt(firstIndexRemoved);
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
		}
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
