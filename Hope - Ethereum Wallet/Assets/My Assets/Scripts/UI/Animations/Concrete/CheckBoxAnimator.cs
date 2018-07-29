using UnityEngine;
using UnityEngine.UI;

public sealed class CheckBoxAnimator : MonoBehaviour
{
	private Button checkBoxButton;
	private GameObject checkMarkIcon;

    public bool ToggledOn { get; set; }

    private void Awake()
	{
		checkBoxButton = transform.GetComponent<Button>();
		checkBoxButton.onClick.AddListener(CheckBoxClicked);
		checkMarkIcon = transform.GetChild(0).gameObject;
        ToggledOn = checkMarkIcon.transform.localScale.x > 0;
	}

	private void CheckBoxClicked()
	{
		checkMarkIcon.AnimateGraphicAndScale(ToggledOn ? 0f : 1f, ToggledOn ? 0f : 1f, 0.15f);
        ToggledOn = !ToggledOn;
	}
}