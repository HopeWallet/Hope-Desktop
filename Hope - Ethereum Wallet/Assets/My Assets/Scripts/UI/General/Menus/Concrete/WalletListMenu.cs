using Hope.Security.ProtectedTypes.Types;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which will handle the entering of a password for unlocking a wallet.
/// </summary>
public class WalletListMenu : Menu<WalletListMenu>
{

    public Button backButton;
    public Button newWalletButton;

    private WalletButton.Factory walletButtonFactory;
    private DynamicDataCache dynamicDataCache;
    private Settings settings;

    /// <summary>
    /// Adds the dependencies required for this menu.
    /// </summary>
    /// <param name="walletButtonFactory"> The factory for creating WalletButtons. </param>
    /// <param name="dynamicDataCache"> The active ByteDataCache. </param>
    /// <param name="settings"> The settings of this menu. </param>
    [Inject]
    public void Construct(WalletButton.Factory walletButtonFactory, DynamicDataCache dynamicDataCache, Settings settings)
    {
        this.walletButtonFactory = walletButtonFactory;
        this.dynamicDataCache = dynamicDataCache;
        this.settings = settings;
    }

    protected override void OnAwake()
    {
        List<GameObject> walletObjects = new List<GameObject>();

        for (int i = 1; i <= SecurePlayerPrefs.GetInt("wallet_count"); i++)
            walletObjects.Add(walletButtonFactory.Create().SetButtonInfo(new WalletInfo(SecurePlayerPrefs.GetString("wallet_" + i + "_name"), i)).gameObject);

        (Animator as WalletListMenuAnimator).Wallets = walletObjects.ToArray();
    }

    /// <summary>
    /// Adds the button click events on start.
    /// </summary>
    private void Start()
    {
        newWalletButton.onClick.AddListener(CreateNewWallet);
    }

    private void CreateNewWallet()
    {
        uiManager.OpenMenu<CreateWalletMenu>();
    }

    /// <summary>
    /// Loads a wallet with the text input by the user as the password.
    /// Will not close this gui or open the next gui unless the password was correct.
    /// </summary>
    //public override void LoadWallet()
    //{
    // set dynamic data cache pass and wallet
    //dynamicDataCache.SetData("pass", new ProtectedString("..."));
    //dynamicDataCache.SetData("walletnum", 1);
    //userWalletManager.UnlockWallet();
    //}

    [Serializable]
    public class Settings
    {
        public Transform walletButtonSpawnTransform;
    }
}
