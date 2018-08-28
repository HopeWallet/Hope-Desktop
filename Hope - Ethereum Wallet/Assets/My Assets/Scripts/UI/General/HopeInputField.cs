using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HopeInputField : MonoBehaviour
{
	public event Action OnInputUpdated;

	[SerializeField] private GameObject placeholder;
	[SerializeField] private GameObject eye;
	[SerializeField] private GameObject errorIcon;
	[SerializeField] private bool noSpaces;
	[SerializeField] private bool placeholderFadeAway;

	public InputField inputFieldBase;

	public TextMeshProUGUI errorMessage;

	private Sprite eyeInactiveNormal;
	private Sprite eyeActiveNormal;

	private string text;

	public string Text
	{
		get { return text; }
		set
		{
			text = value;
			inputFieldBase.text = text;
		}
	}

	public bool Error { get; set; }

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
	/// Sets the target sprite from a given string
	/// </summary>
	/// <param name="targetSprite"> The target sprite being set </param>
	/// <param name="iconName"> The name of the icon file </param>
	private void SetSprite(ref Sprite targetSprite, string iconName)
	{
		Texture2D loadedTexture = Resources.Load("UI/Graphics/Textures/New/Icons/" + iconName) as Texture2D;
		targetSprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
	}

	/// <summary>
	/// Updates the visuals of the input field
	/// </summary>
	/// <param name="emptyString"> Whether the string is empty or not </param>
	public void UpdateVisuals()
	{
		bool emptyString = string.IsNullOrEmpty(Text);

		if (placeholder != null)
		{
			if (placeholderFadeAway)
				placeholder.AnimateColor(emptyString ? UIColors.LightGrey : new Color(1f, 1f, 1f, 0f), 0.15f);
			else
				placeholder.AnimateTransformY(emptyString ? 0f : 35f, 0.15f);
		}
		inputFieldBase.gameObject.AnimateColor(emptyString ? UIColors.White : Error ? UIColors.Red : UIColors.Green, 0.15f);
		if (errorIcon != null) errorIcon.AnimateGraphic(emptyString ? 0f : Error ? 1f : 0f, 0.15f);
		if (errorMessage != null) errorMessage.gameObject.AnimateGraphic(emptyString ? 0f : Error ? 1f : 0f, 0.15f);
	}

	/// <summary>
	/// The input field is changed
	/// </summary>
	/// <param name="inputString"> The text in the input field s</param>
	private void InputFieldChanged(string inputString)
	{
		if (noSpaces)
			Text = inputString.Trim();
		else
			Text = inputString;

		OnInputUpdated?.Invoke();

		bool emptyString = string.IsNullOrEmpty(Text);

		UpdateVisuals();

		if (eye != null)
			eye.AnimateGraphicAndScale(emptyString ? 0f : 1f, emptyString ? 0f : 1f, 0.1f);
	}

	/// <summary>
	/// The eye icon is clicked and either enables or disables vision of the password
	/// </summary>
	private void EyeClicked()
	{
		if (inputFieldBase.contentType == InputField.ContentType.Password)
		{
			inputFieldBase.contentType = InputField.ContentType.Standard;
			inputFieldBase.textComponent.text = inputFieldBase.text;
		}
		else
		{
			inputFieldBase.contentType = InputField.ContentType.Password;
			inputFieldBase.textComponent.text = string.Empty;
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
