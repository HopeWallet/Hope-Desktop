using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which is used as a check box.
/// </summary>
public sealed class CheckBox : MonoBehaviour
{
    public event Action<bool> OnValueChanged;

	private Button checkBoxButton;
	private GameObject checkMarkIcon;

    /// <summary>
    /// Whether the checkbox is toggled on or not.
    /// </summary>
    public bool ToggledOn { get; set; }

    /// <summary>
    /// Initializes the CheckBox.
    /// </summary>
    private void Awake()
	{
		checkBoxButton = transform.GetComponent<Button>();
		checkBoxButton.onClick.AddListener(OnCheckBoxClicked);
		checkMarkIcon = transform.GetChild(0).gameObject;
        ToggledOn = checkMarkIcon.transform.localScale.x > 0;
	}

    /// <summary>
    /// Toggles the checkbox on/off without animation.
    /// </summary>
    /// <param name="toggledOn"> Whether it should be toggled on or off. </param>
    public void Toggle(bool toggledOn)
    {
		ToggledOn = toggledOn;
		gameObject.GetComponent<Image>().color = ToggledOn ? UIColors.Green : UIColors.Blue;
        checkMarkIcon.transform.localScale = ToggledOn ? Vector2.one : Vector2.zero;
        checkMarkIcon.GetComponent<Image>().color = ToggledOn ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
    }

    /// <summary>
    /// Called when the checkbox is clicked.
    /// </summary>
	private void OnCheckBoxClicked()
	{
		ToggledOn = !ToggledOn;
		checkMarkIcon.AnimateGraphicAndScale(ToggledOn ? 1f : 0f, ToggledOn ? 1f : 0f, 0.15f);
		gameObject.AnimateColor(ToggledOn ? UIColors.Green : UIColors.Blue, 0.15f);

        OnValueChanged?.Invoke(ToggledOn);
	}
}