using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which manages the opened wallet ui.
/// </summary>
public sealed class OpenWalletMenu : Menu<OpenWalletMenu>
{
    public GameObject lockPurposeSection;

	public TMP_Text assetText,
					balanceText,
					currentTokenNetWorthText;

    public Image assetImage;

    public DropdownButton optionsDropdownButton;

    private TokenContractManager tokenContractManager;
    private TradableAssetManager tradableAssetManager;
    private PRPS prpsContract;

    private const int MAX_ASSET_NAME_LENGTH = 36;
    private const int MAX_ASSET_BALANCE_LENGTH = 54;

    /// <summary>
    /// Injects the required dependency into this class.
    /// </summary>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="prpsContract"> The active PRPS contract. </param>
    /// <param name="uiSettings"> The ui settings. </param>
    [Inject]
    public void Construct(
		TokenContractManager tokenContractManager,
		TradableAssetManager tradableAssetManager,
        PRPS prpsContract,
		UIManager.Settings uiSettings)
    {
        this.tokenContractManager = tokenContractManager;
        this.tradableAssetManager = tradableAssetManager;
        this.prpsContract = prpsContract;

        optionsDropdownButton.dropdownButtons = uiSettings.generalSettings.dropdowns.optionsDropdowns;
    }

    /// <summary>
    /// Initializes the instance and the wallet parent object.
    /// </summary>
    private void Start()
    {
        TradableAssetManager.OnBalancesUpdated += UpdateAssetUI;
        tokenContractManager.StartTokenLoad(OpenMenu);
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
    }

    public override void GoBack()
    {
        // Logout popup
    }
}