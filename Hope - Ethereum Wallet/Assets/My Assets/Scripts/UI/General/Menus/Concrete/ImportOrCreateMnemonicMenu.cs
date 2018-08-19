﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which allows the user to choose whether to import or create a new wallet.
/// </summary>
public class ImportOrCreateMnemonicMenu : Menu<ImportOrCreateMnemonicMenu>
{
	[SerializeField] private Button importButton, createButton;

    /// <summary>
    /// Adds the click events for the buttons of this menu.
    /// </summary>
    private void Start()
    {
        importButton.onClick.AddListener(ImportWallet);
        createButton.onClick.AddListener(CreateWallet);
    }

	/// <summary>
	/// Opens the exit confirmation popup and enables the note text
	/// </summary>
	protected override void OpenExitConfirmationPopup() => popupManager.GetPopup<ExitConfirmationPopup>(true)?.SetDetails(true);

	/// <summary>
	/// Changes to the import wallet gui.
	/// </summary>
	public void ImportWallet() => uiManager.OpenMenu<ImportMnemonicMenu>();

    /// <summary>
    /// Changes to the create wallet gui.
    /// </summary>
    public void CreateWallet() => uiManager.OpenMenu<CreateMnemonicMenu>();

}
