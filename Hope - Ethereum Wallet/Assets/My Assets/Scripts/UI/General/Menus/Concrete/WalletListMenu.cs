using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which will handle the entering of a password for unlocking a wallet.
/// </summary>
public sealed class WalletListMenu : Menu<WalletListMenu>
{
	public event Action<bool> BottomButtonsVisible;
	public static Action PopupClosed;

	[SerializeField] private Button newWalletButton;

	private WalletButton.Factory walletButtonFactory;
    private DynamicDataCache dynamicDataCache;
	private HopeWalletInfoManager walletInfoManager;
	private HopeWalletInfoManager.Settings walletSettings;
    private Settings settings;

    /// <summary>
    /// Adds the dependencies required for this menu.
    /// </summary>
    /// <param name="walletButtonFactory"> The factory for creating WalletButtons. </param>
    /// <param name="dynamicDataCache"> The active ByteDataCache. </param>
    /// <param name="walletSettings"> The settings for the UserWallet. </param> 
    /// <param name="settings"> The settings of this menu. </param>
    [Inject]
    public void Construct(
        WalletButton.Factory walletButtonFactory,
        DynamicDataCache dynamicDataCache,
		HopeWalletInfoManager walletInfoManager,
        HopeWalletInfoManager.Settings walletSettings,
        Settings settings)
    {
        this.walletButtonFactory = walletButtonFactory;
        this.dynamicDataCache = dynamicDataCache;
		this.walletInfoManager = walletInfoManager;
        this.walletSettings = walletSettings;
        this.settings = settings;
    }

    /// <summary>
    /// Creates WalletButtons for each wallet that exists currently in the PlayerPrefs.
    /// </summary>
    protected override void OnAwake()
    {
        List<GameObject> walletObjects = new List<GameObject>();

        for (int i = 1; i <= SecurePlayerPrefs.GetInt(walletSettings.walletCountPrefName); i++)
        {
            walletObjects.Add(walletButtonFactory.Create()
                         .SetButtonInfo(new WalletInfo(SecurePlayerPrefs.GetString(walletSettings.walletNamePrefName + i), null, i)).gameObject.transform.GetChild(0).gameObject);
        } (Animator as WalletListMenuAnimator).Wallets = walletObjects.ToArray();
	}

	/// <summary>
	/// Adds the button click events on start.
	/// </summary>
	private void Start() => newWalletButton.onClick.AddListener(CreateNewWallet);

	/// <summary>
	/// Opens the CreateWalletMenu to allow for creating a new wallet.
	/// </summary>
	private void CreateNewWallet() => uiManager.OpenMenu<CreateWalletMenu>();

	/// <summary>
	/// The settings for this WalletListMenu.
	/// </summary>
	[Serializable]
    public class Settings
    {
        public Transform walletButtonSpawnTransform;
    }
}
