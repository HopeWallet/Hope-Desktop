using UnityEngine;
using UnityEngine.UI;

public class WalletCreatedMenu : Menu<WalletCreatedMenu>
{
    [SerializeField] private Button openWalletButton;

    private void Start() => openWalletButton.onClick.AddListener(() => uiManager.OpenMenu<OpenWalletMenu>());
}