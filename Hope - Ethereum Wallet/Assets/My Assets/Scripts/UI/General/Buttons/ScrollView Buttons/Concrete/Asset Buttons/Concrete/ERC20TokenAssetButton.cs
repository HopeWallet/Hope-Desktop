﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which manages the buttons for ERC20 token assets.
/// </summary>
public sealed class ERC20TokenAssetButton : TradableAssetButton<ERC20TokenAssetButton>
{
	[SerializeField] private GameObject removeButton;

	private Button removeButtonComponent;

	private bool hovering, removingAssetPopupOpen;

	/// <summary>
	/// Sets the button component variable and button click listener
	/// </summary>
	protected override void OnAwake()
	{
		removeButtonComponent = removeButton.GetComponent<Button>();
		removeButtonComponent.onClick.AddListener(RemoveButtonClicked);
	}

	/// <summary>
	/// The symbol display text for the ERC20TokenAsset.
	/// </summary>
	protected override string AssetDisplayText => ButtonInfo.AssetSymbol.LimitEnd(5, "...");

	/// <summary>
	/// The balance display text for the ERC20TokenAsset.
	/// </summary>
	protected override string AssetBalanceText => StringUtils.LimitEnd(ButtonInfo.AssetBalance.ToString(), 7, "...");

	/// <summary>
	/// Called when the pointer hover state has been changed.
	/// </summary>
	/// <param name="mouseHovering"> Whether the mouse is hovering over this object or not. </param>
	public override void ButtonHovered(bool mouseHovering)
	{
		hovering = mouseHovering;

		if (!removingAssetPopupOpen)
			removeButton.AnimateGraphicAndScale(mouseHovering ? 1f : 0f, mouseHovering ? 1f : 0f, 0.15f);
	}

	/// <summary>
	/// Opens the remove token confirmation popup
	/// </summary>
	private void RemoveButtonClicked()
	{
		removingAssetPopupOpen = true;
		removeButtonComponent.interactable = false;
		popupManager.GetPopup<GeneralOkCancelPopup>()
			.SetSubText("Are you sure you want to remove " + symbolText.text + "?")
			.OnOkClicked(() => Debug.Log("Remove token"))
			.OnFinish(PopupClosed);
	}

	/// <summary>
	/// The remove token confirmation popup has been closed
	/// </summary>
	private void PopupClosed()
	{
		removeButtonComponent.interactable = true;
		removingAssetPopupOpen = false;
		ButtonHovered(hovering);
	}
}