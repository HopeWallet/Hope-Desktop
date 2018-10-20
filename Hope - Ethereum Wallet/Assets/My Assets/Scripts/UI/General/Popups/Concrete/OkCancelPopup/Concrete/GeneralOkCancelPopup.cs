using System;
using TMPro;
using UnityEngine;
using Zenject;

/// <summary>
/// Class that manages a general ok/cancel popup
/// </summary>
public sealed class GeneralOkCancelPopup : OkCancelPopupComponent<GeneralOkCancelPopup>, IEnterButtonObservable
{
	[SerializeField] private TextMeshProUGUI subText;

	private bool disableEnterButton;

	private Action onFinish;

	private ButtonClickObserver buttonClickObserver;

	/// <summary>
	/// Sets the required dependency and subscribes this button click observer
	/// </summary>
	/// <param name="buttonClickObserver"> The active ButtonClickObserver </param>
	[Inject]
	public void Construct(ButtonClickObserver buttonClickObserver)
	{
		this.buttonClickObserver = buttonClickObserver;

		buttonClickObserver.SubscribeObservable(this);
	}

	/// <summary>
	/// Unsubscribes this buttonClickObserver
	/// </summary>
	private void OnDisable() => buttonClickObserver.UnsubscribeObservable(this);

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
	/// Disables the ability to hit the enter button and click yes
	/// </summary>
	/// <returns> The instance of the class </returns>
	public GeneralOkCancelPopup DisableEnterButton()
	{
		disableEnterButton = false;
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

	/// <summary>
	/// User has hit the enter button
	/// </summary>
	/// <param name="clickType"> The ClickType </param>
	public void EnterButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down || disableEnterButton)
			return;

		okButton.Press();
	}
}
