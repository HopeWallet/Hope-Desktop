using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HopeInputField : MonoBehaviour
{
	[SerializeField] private GameObject placeholder;
	[SerializeField] private InputField inputFieldBase;
	[SerializeField] private GameObject eye;
	[SerializeField] private GameObject errorIcon;

	public TextMeshProUGUI ErrorMessage;

	private Sprite eyeInactive;
	private Sprite eyeActive;

	private bool error;

	public bool Error
	{
		get { return error; }
		set
		{
			error = value;

			errorIcon.AnimateGraphic(error ? 1f : 0f, 0.1f);
			ErrorMessage.gameObject.AnimateGraphic(error ? 1f : 0f, 0.1f);
		}
	}

	private void Awake()
	{
		inputFieldBase.onValueChanged.AddListener(InputFieldChanged);

		if (eye != null)
		{
			eye.GetComponent<Button>().onClick.AddListener(EyeClicked);

			var inactive = Resources.Load("UI/Graphics/Textures/New/Icons/Eye_Inactive") as Texture2D;
			var active = Resources.Load("UI/Graphics/Textures/New/Icons/Eye_Active") as Texture2D;

			eyeInactive = Sprite.Create(inactive, new Rect(0f, 0f, inactive.width, inactive.height), new Vector2(0.5f, 0.5f));
			eyeActive = Sprite.Create(active, new Rect(0f, 0f, active.width, active.height), new Vector2(0.5f, 0.5f));
		}
	}

	private void InputFieldChanged(string str)
	{
		bool emptyString = string.IsNullOrEmpty(str);

		if (!error)
		{
			placeholder.AnimateTransformY(emptyString ? 0f : 35f, 0.1f);
			inputFieldBase.gameObject.AnimateColor(emptyString ? new Color(0.85f, 0.85f, 0.85f) : UIColors.Green, 0.1f);
		}

		if (eye != null)
			eye.SetActive(!emptyString);
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

		eye.GetComponent<Image>().sprite = inputFieldBase.contentType == InputField.ContentType.Password ? eyeInactive : eyeActive;
	}
}
