using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which manages the opened wallet ui.
/// </summary>
public sealed class OpenWalletMenu : Menu<OpenWalletMenu>
{
    public static event Action<TabType> OnTabChanged;

    public GameObject lockPurposeSection,
                      lockPurposeNotificationSection;

    public TMP_Text assetText,
                    balanceText,
                    currentTokenNetWorthText,
                    lockPrpsNotificationText;

    public Image assetImage;

    private TokenContractManager tokenContractManager;
    private TradableAssetManager tradableAssetManager;
    private TradableAssetNotificationManager notificationManager;
    private LockedPRPSManager lockedPrpsManager;
    private PRPS prpsContract;

    private const int MAX_ASSET_NAME_LENGTH = 36;
    private const int MAX_ASSET_BALANCE_LENGTH = 54;

	private bool idle;

	private Vector3 previousMousePosition;

	private int idleTime;

    /// <summary>
    /// Injects the required dependency into this class.
    /// </summary>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="notificationManager"> The active TradableAssetNotificationManager. </param>
    /// <param name="lockedPrpsManager"> The active LockedPRPSManager. </param>
    /// <param name="prpsContract"> The active PRPS contract. </param>
    /// <param name="uiSettings"> The ui settings. </param>
    [Inject]
    public void Construct(
        TokenContractManager tokenContractManager,
        TradableAssetManager tradableAssetManager,
        TradableAssetNotificationManager notificationManager,
        LockedPRPSManager lockedPrpsManager,
        PRPS prpsContract,
        UIManager.Settings uiSettings)
    {
        this.tokenContractManager = tokenContractManager;
        this.tradableAssetManager = tradableAssetManager;
        this.notificationManager = notificationManager;
        this.lockedPrpsManager = lockedPrpsManager;
        this.prpsContract = prpsContract;
    }

    /// <summary>
    /// Initializes the instance and the wallet parent object.
    /// </summary>
    private void Start()
    {
        TradableAssetManager.OnBalancesUpdated += UpdateAssetUI;
        lockedPrpsManager.OnLockedPRPSUpdated += UpdateAssetNotifications;
        tokenContractManager.StartTokenLoad(OpenMenu);

		idleTime = 0;
		//StartCoroutine(CheckIfStillIdle);
    }

	private void Update()
	{
		idle = previousMousePosition == Input.mousePosition;

		previousMousePosition = Input.mousePosition;
	}

	private IEnumerator CheckIfStillIdle()
	{
		yield return new WaitForSeconds(1);

		if (idle)
			idleTime += 1;
	}

	/// <summary>
	/// Called when the OpenWalletMenu is first opened.
	/// </summary>
	private void OpenMenu() => transform.GetChild(0).gameObject.SetActive(true);

	/// <summary>
	/// Updates the ui for the newest TradableAsset.
	/// </summary>
	public void UpdateAssetUI()
    {
        var tradableAsset = tradableAssetManager.ActiveTradableAsset;

        if (tradableAsset == null)
            return;

        string assetBalance = tradableAsset.AssetBalance.ToString();

        lockPurposeSection.SetActive(tradableAsset.AssetAddress.EqualsIgnoreCase(prpsContract.ContractAddress));

        assetText.text = tradableAsset.AssetName.LimitEnd(MAX_ASSET_NAME_LENGTH, "...");
        balanceText.text = assetBalance.LimitEnd(MAX_ASSET_BALANCE_LENGTH, "...");

        assetImage.sprite = tradableAsset.AssetImage;

        UpdateAssetNotifications();
    }

    /// <summary>
    /// Updates the notifications for locked purpose and saves the transaction count of the current asset.
    /// </summary>
    private void UpdateAssetNotifications()
    {
        var lockedPrpsCount = lockedPrpsManager.UnlockableItems.Count;
        lockPurposeNotificationSection.SetActive(lockedPrpsCount > 0);
        lockPrpsNotificationText.text = lockedPrpsCount.ToString();
        lockPrpsNotificationText.fontSize = lockedPrpsCount.ToString().Length > 1 ? 15 : 19;

        notificationManager.SaveTransactionCount(tradableAssetManager.ActiveTradableAsset.AssetAddress);
    }

    public override void GoBack()
    {
        // Logout popup
    }

    public enum TabType { All, Sent, Received }
}