using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which is used as a check box.
/// </summary>
public sealed class CheckBox : MonoBehaviour
{
    public event Action<bool> OnCheckboxClicked;

	private Button checkBoxButton;
	private GameObject checkMarkIcon;

    /// <summary>
    /// Whether the checkbox is toggled on or not.
    /// </summary>
    public bool IsToggledOn { get; private set; }

    /// <summary>
    /// Initializes the CheckBox.
    /// </summary>
    private void Awake()
	{
		checkBoxButton = transform.GetComponent<Button>();
		checkBoxButton.onClick.AddListener(CheckboxClicked);
		checkMarkIcon = transform.GetChild(0).gameObject;
        IsToggledOn = checkMarkIcon.transform.localScale.x > 0;
	}

    /// <summary>
    /// Toggles the checkbox on/off without animation.
    /// </summary>
    /// <param name="isToggledOn"> Whether it should be toggled on or off. </param>
    public void SetValue(bool isToggledOn)
    {
		IsToggledOn = isToggledOn;
		gameObject.GetComponent<Image>().color = IsToggledOn ? UIColors.Green : UIColors.Blue;
        checkMarkIcon.transform.localScale = IsToggledOn ? Vector2.one : Vector2.zero;
        checkMarkIcon.GetComponent<Image>().color = IsToggledOn ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
    }

	/// <summary>
	/// Checkbox is clicked
	/// </summary>
	private void CheckboxClicked()
	{
		IsToggledOn = !IsToggledOn;
		ToggleCheckbox(IsToggledOn);
		OnCheckboxClicked?.Invoke(IsToggledOn);
	}

    /// <summary>
    /// Animates the checkmark icon and the box colour
    /// </summary>
	public void ToggleCheckbox(bool isToggledOn)
	{
		IsToggledOn = isToggledOn;
		checkMarkIcon.AnimateGraphicAndScale(IsToggledOn ? 1f : 0f, IsToggledOn ? 1f : 0f, 0.15f);
		gameObject.AnimateColor(IsToggledOn ? UIColors.Green : UIColors.Blue, 0.15f);
	}
}