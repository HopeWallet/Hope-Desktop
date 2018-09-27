using System;
using TMPro;
using UnityEngine;

public class GeneralOkCancelPopup : OkCancelPopupComponent<GeneralOkCancelPopup>
{
	[SerializeField] private TextMeshProUGUI subText;

	private Action onFinish;

	/// <summary>
	/// Sets the subText text element
	/// </summary>
	/// <param name="subText"> The given string </param>
	/// <returns> The instance of the class </returns>
	public GeneralOkCancelPopup SetSubText(string subText)
	{
		this.subText.text = subText;
		return this;
	}

	/// <summary>
	/// Sets the action to be called when the ok button is clicked
	/// </summary>
	/// <param name="onOkClicked"> The given action </param>
	/// <returns> The instance of the class </returns>
	public GeneralOkCancelPopup OnOkClicked(Action onOkClicked)
	{
		okButton.onClick.AddListener(() => onOkClicked());
		return this;
	}

	/// <summary>
	/// Sets the ok button to uninteractable
	/// </summary>
	/// <returns> The instance of the class </returns>
	public GeneralOkCancelPopup DisableOkButton()
	{
		okButton.interactable = false;
		return this;
	}

	/// <summary>
	/// Sets the final action to be called when the popup is destroyed
	/// </summary>
	/// <param name="onFinish"> The final action </param>
	public void OnFinish(Action onFinish) => this.onFinish = onFinish;

	/// <summary>
	/// Calls the onFinish action if it has been set
	/// </summary>
	private void OnDestroy() => onFinish?.Invoke();
}
