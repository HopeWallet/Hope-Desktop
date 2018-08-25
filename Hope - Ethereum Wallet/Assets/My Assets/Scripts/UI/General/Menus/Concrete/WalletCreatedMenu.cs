using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class used for displaying a menu which shows the wallet was created.
/// </summary>
public class WalletCreatedMenu : Menu<WalletCreatedMenu>
{
    [SerializeField] private Button openWalletButton;

    /// <summary>
    /// Adds the listener to the openWalletButton which opens the OpenWalletMenu.
    /// </summary>
    private void Start() => openWalletButton.onClick.AddListener(() => uiManager.OpenMenu<OpenWalletMenu>());
}