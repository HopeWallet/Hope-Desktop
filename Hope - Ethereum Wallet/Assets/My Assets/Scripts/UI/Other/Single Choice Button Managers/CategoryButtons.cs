using TMPro;
using UnityEngine;

public class CategoryButtons : SingleChoiceButtonsBase
{
	/// <summary>
	/// Changes the visuals of the newly active, and previously active radio button
	/// </summary>
	/// <param name="activeButton"> the index of the button being changed </param>
	/// <param name="active"> Whether the button is currently active or not </param>
	protected override void SetRadioButtonVisuals(int activeButton, bool active)
	{
		base.SetRadioButtonVisuals(activeButton, active);

		Transform ButtonTransform = transform.GetChild(activeButton);

		ButtonTransform.GetComponent<TextMeshProUGUI>().color = active ? UIColors.Green : UIColors.White;
		ButtonTransform.GetChild(0).gameObject.AnimateGraphic(active ? 1f : 0f, 0.15f);
	}
}
