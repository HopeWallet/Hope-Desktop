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
    public bool ToggledOn { get; set; }

    /// <summary>
    /// Initializes the CheckBox.
    /// </summary>
    private void Awake()
	{
		checkBoxButton = transform.GetComponent<Button>();
		checkBoxButton.onClick.AddListener(CheckboxClicked);
		checkMarkIcon = transform.GetChild(0).gameObject;
        ToggledOn = checkMarkIcon.transform.localScale.x > 0;
	}

    /// <summary>
    /// Toggles the checkbox on/off without animation.
    /// </summary>
    /// <param name="toggledOn"> Whether it should be toggled on or off. </param>
    public void SetCheckboxValue(bool toggledOn)
    {
		ToggledOn = toggledOn;
		gameObject.GetComponent<Image>().color = ToggledOn ? UIColors.Green : UIColors.Blue;
        checkMarkIcon.transform.localScale = ToggledOn ? Vector2.one : Vector2.zero;
        checkMarkIcon.GetComponent<Image>().color = ToggledOn ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
    }

	/// <summary>
	/// Checkbox is clicked
	/// </summary>
	private void CheckboxClicked()
	{
		ToggledOn = !ToggledOn;
		AnimateElements(ToggledOn);
		OnCheckboxClicked?.Invoke(ToggledOn);
	}

    /// <summary>
    /// Animates the checkmark icon and the box colour
    /// </summary>
	public void AnimateElements(bool toggledOn)
	{
		ToggledOn = toggledOn;
		checkMarkIcon.AnimateGraphicAndScale(ToggledOn ? 1f : 0f, ToggledOn ? 1f : 0f, 0.15f);
		gameObject.AnimateColor(ToggledOn ? UIColors.Green : UIColors.Blue, 0.15f);
	}
}