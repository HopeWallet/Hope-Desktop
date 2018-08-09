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

    }

    /// <summary>
    /// Called when the checkbox is clicked.
    /// </summary>
	private void OnCheckBoxClicked()
	{
		checkMarkIcon.AnimateGraphicAndScale(ToggledOn ? 0f : 1f, ToggledOn ? 0f : 1f, 0.15f);
        ToggledOn = !ToggledOn;

        OnValueChanged?.Invoke(ToggledOn);
	}
}