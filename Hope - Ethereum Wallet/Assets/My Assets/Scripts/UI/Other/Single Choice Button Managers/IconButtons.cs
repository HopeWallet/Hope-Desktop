using TMPro;

/// <summary>
/// Class that manages the visuals of changing different radio buttons
/// </summary>
public sealed class IconButtons : SingleChoiceButtonsBase
{
	public bool whiteToGreen;

	/// <summary>
	/// Changes the visuals of the newly active, and previously active radio button
	/// </summary>
	/// <param name="buttonNum"> the index of the button being changed </param>
	/// <param name="active"> Whether the button is currently active or not </param>
	protected override void SetButtonVisuals(int buttonNum, bool active)
	{
		base.SetButtonVisuals(buttonNum, active);

		if (whiteToGreen)
			transform.GetChild(buttonNum).GetChild(0).gameObject.AnimateColor(active ? UIColors.Green : UIColors.White, 0.15f);
		else
			transform.GetChild(buttonNum).GetChild(0).gameObject.AnimateColor(active ? UIColors.White : UIColors.LightGrey, 0.15f);
	}
}
