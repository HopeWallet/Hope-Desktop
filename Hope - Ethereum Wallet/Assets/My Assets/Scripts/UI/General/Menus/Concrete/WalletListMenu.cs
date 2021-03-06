﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which will handle the entering of a password for unlocking a wallet.
/// </summary>
public sealed class WalletListMenu : Menu<WalletListMenu>
{
	public static Action PopupClosed;

	[SerializeField] private Button newWalletButton;
	[SerializeField] private GameObject hopeLogo;

	private WalletButton.Factory walletButtonFactory;
    private DynamicDataCache dynamicDataCache;
	private HopeWalletInfoManager hopeWalletInfoManager;
    private Settings settings;

    /// <summary>
    /// The list of wallet objects.
    /// </summary>
    public List<GameObject> Wallets { get; } = new List<GameObject>();

    /// <summary>
    /// Adds the dependencies required for this menu.
    /// </summary>
    /// <param name="walletButtonFactory"> The factory for creating WalletButtons. </param>
    /// <param name="dynamicDataCache"> The active ByteDataCache. </param>
    /// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager. </param>
    /// <param name="settings"> The settings of this menu. </param>
    [Inject]
    public void Construct(
        WalletButton.Factory walletButtonFactory,
        DynamicDataCache dynamicDataCache,
		HopeWalletInfoManager hopeWalletInfoManager,
        Settings settings)
    {
        this.walletButtonFactory = walletButtonFactory;
        this.dynamicDataCache = dynamicDataCache;
		this.hopeWalletInfoManager = hopeWalletInfoManager;
        this.settings = settings;
    }

    /// <summary>
    /// Creates WalletButtons for each wallet that is currently saved.
    /// </summary>
    private void OnEnable()
    {
        for (int i = 1; i <= hopeWalletInfoManager.WalletCount; i++)
        {
            Wallets.Add(walletButtonFactory.Create()
                   .SetButtonInfo(hopeWalletInfoManager.GetWalletInfo(i)).gameObject.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// Gets rid of all created wallet objects.
    /// </summary>
    private void OnDisable()
    {
        for (int i = 0; i < Wallets.Count; i++)
            Destroy(Wallets[i].transform.parent.gameObject);

        Wallets.Clear();
    }

    /// <summary>
    /// Adds the button click events on start.
    /// </summary>
    private void Start()
    {
        newWalletButton.onClick.AddListener(CreateNewWallet);
    }

	/// <summary>
	/// Animates the back button and hope logo out if the user goes back to the ChooseWalletMenu
	/// </summary>
	protected override void OnBackPressed()
	{
		base.OnBackPressed();

		backButton.gameObject.AnimateGraphicAndScale(0f, 0f, 0.3f);
		hopeLogo.AnimateGraphicAndScale(0f, 0f, 0.3f);
	}

	/// <summary>
	/// Opens the CreateWalletMenu to allow for creating a new wallet.
	/// </summary>
	private void CreateNewWallet()
    {
		uiManager.OpenMenu<CreateWalletMenu>();
    }

    /// <summary>
    /// The settings for this WalletListMenu.
    /// </summary>
    [Serializable]
    public class Settings
    {
        public Transform walletButtonSpawnTransform;
    }
}
