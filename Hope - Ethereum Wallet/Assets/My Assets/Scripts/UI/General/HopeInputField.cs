using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HopeInputField : MonoBehaviour
{
	public event Action OnInputUpdated;

	[SerializeField] private GameObject placeholder;
	[SerializeField] private InputField inputFieldBase;
	[SerializeField] private GameObject eye;
	[SerializeField] private GameObject errorIcon;

	public TextMeshProUGUI errorMessage;

	private Sprite eyeInactiveNormal;
	private Sprite eyeActiveNormal;

	public string Input { get; private set; }

	public bool Error { get; set; }

	private void Awake()
	{
		inputFieldBase.onValueChanged.AddListener(InputFieldChanged);

		if (eye != null)
		{
			eye.GetComponent<Button>().onClick.AddListener(EyeClicked);

			SetSprite(ref eyeInactiveNormal, "Eye_Inactive_Normal");
			SetSprite(ref eyeActiveNormal, "Eye_Active_Normal");
		}

		Error = true;
	}

	private void SetSprite(ref Sprite targetSprite, string iconName)
	{
		Texture2D loadedTexture = Resources.Load("UI/Graphics/Textures/New/Icons/" + iconName) as Texture2D;
		targetSprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
	}

	public void UpdateVisuals(bool emptyString)
	{
		placeholder.AnimateTransformY(emptyString ? 0f : 35f, 0.1f);
		inputFieldBase.gameObject.AnimateColor(emptyString ? new Color(0.85f, 0.85f, 0.85f) : Error ? UIColors.Red : UIColors.Green, 0.1f);
		errorIcon.AnimateGraphic(emptyString ? 0f : Error ? 1f : 0f, 0.1f);
		errorMessage.gameObject.AnimateGraphic(emptyString ? 0f : Error ? 1f : 0f, 0.1f);
	}

	private void InputFieldChanged(string inputString)
	{
		Input = inputString;
		OnInputUpdated?.Invoke();

		bool emptyString = string.IsNullOrEmpty(inputString);

		UpdateVisuals(emptyString);

		if (eye != null)
			eye.AnimateGraphicAndScale(emptyString ? 0f : 1f, emptyString ? 0f : 1f, 0.1f);
	}

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
}
