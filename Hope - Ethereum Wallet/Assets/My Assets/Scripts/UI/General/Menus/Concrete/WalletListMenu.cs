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
	[SerializeField] private Button deleteButton, editButton, signInButton, newWalletButton;

	[SerializeField] private Transform walletList;

	private int activeWalletNum = 1;

	private WalletListMenuAnimator walletListMenuAnimator;
	private WalletButton.Factory walletButtonFactory;
    private DynamicDataCache dynamicDataCache;
    private UserWalletInfoManager.Settings walletSettings;
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
        UserWalletInfoManager.Settings walletSettings,
        Settings settings)
    {
        this.walletButtonFactory = walletButtonFactory;
        this.dynamicDataCache = dynamicDataCache;
        this.walletSettings = walletSettings;
        this.settings = settings;
    }

    /// <summary>
    /// Creates WalletButtons for each wallet that exists currently in the PlayerPrefs.
    /// </summary>
    protected override void OnAwake()
    {
		walletListMenuAnimator = transform.GetComponent<WalletListMenuAnimator>();

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
	private void Start()
	{
		deleteButton.onClick.AddListener(DeleteWallet);
		editButton.onClick.AddListener(EditWallet);
		signInButton.onClick.AddListener(UnlockWallet);
		newWalletButton.onClick.AddListener(CreateNewWallet);
	}

	protected override void OnBackPressed()
	{
		base.OnBackPressed();

		SetActiveButton(walletList.GetChild(activeWalletNum - 1), false);
		activeWalletNum = 1;
		walletListMenuAnimator.BottomButtonsVisible = false;
	}

	/// <summary>
	/// Opens the CreateWalletMenu to allow for creating a new wallet.
	/// </summary>
	private void CreateNewWallet() => uiManager.OpenMenu<CreateWalletMenu>();

	private void DeleteWallet()
	{

	}

	private void EditWallet()
	{

	}

	private void UnlockWallet()
	{
		dynamicDataCache.SetData("walletnum", activeWalletNum);
		popupManager.GetPopup<UnlockWalletPopup>();
	}

	public void SetNewActiveWallet(int newWalletNum)
	{
		if (!walletListMenuAnimator.BottomButtonsVisible)
			walletListMenuAnimator.BottomButtonsVisible = true;

		SetActiveButton(walletList.GetChild(activeWalletNum - 1), false);
		SetActiveButton(walletList.GetChild(newWalletNum - 1), true);
		activeWalletNum = newWalletNum;
	}

	private void SetActiveButton(Transform objectTransform, bool newActiveWallet)
	{
		objectTransform.GetComponent<Button>().interactable = !newActiveWallet;

		float value = newActiveWallet ? 1f : 0.85f;
		objectTransform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(value, value, value);
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
