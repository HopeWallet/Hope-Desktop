
public sealed class GeneralRadioButtons : SingleChoiceButtonsBase
{
	/// <summary>
	/// Changes the visuals of the newly active, and previously active radio button
	/// </summary>
	/// <param name="activeButton"> the index of the button being changed </param>
	/// <param name="active"> Whether the button is currently active or not </param>
	protected override void SetButtonVisuals(int activeButton, bool active)
	{
		base.SetButtonVisuals(activeButton, active);

		for (int i = 0; i < transform.GetChild(activeButton).childCount; i++)
			transform.GetChild(activeButton).GetChild(i).gameObject.AnimateColor(active ? UIColors.White : UIColors.LightGrey, 0.15f);
	}
}
