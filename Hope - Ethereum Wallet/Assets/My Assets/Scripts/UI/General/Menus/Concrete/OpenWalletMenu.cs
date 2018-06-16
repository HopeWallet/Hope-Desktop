using System.Linq;
using UISettings;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which manages the opened wallet ui.
/// </summary>
public class OpenWalletMenu : Menu<OpenWalletMenu>
{

    public Text assetText,
                assetSymbolText,
                smallBalanceText,
                balanceText;

    public Image assetImage,
                 smallAssetImage;

    public DropdownButton optionsDropdownButton;

    private TokenContractManager tokenContractManager;
    private TradableAssetManager tradableAssetManager;
    private Dropdowns uiDropdowns;

    private const int MAX_ASSET_NAME_LENGTH = 36;
    private const int MAX_ASSET_BALANCE_LENGTH = 50;
    private const int MAX_SMALL_ASSET_BALANCE_LENGTH = 18;

    /// <summary>
    /// Injects the required dependency into this class.
    /// </summary>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="uiSettings"> The ui settings. </param>
    [Inject]
    public void Construct(TokenContractManager tokenContractManager, TradableAssetManager tradableAssetManager, UIManager.Settings uiSettings)
    {
        this.tokenContractManager = tokenContractManager;
        this.tradableAssetManager = tradableAssetManager;
        uiDropdowns = uiSettings.generalSettings.dropdowns;
    }

    /// <summary>
    /// Initializes the instance and the wallet parent object.
    /// </summary>
    private void Start()
    {
        TradableAssetManager.OnBalancesUpdated += UpdateAssetUI;
        tokenContractManager.StartTokenLoad(() => transform.GetChild(0).gameObject.SetActive(true));
    }

    /// <summary>
    /// Updates the ui for the newest TradableAsset.
    /// </summary>
    public void UpdateAssetUI()
    {
        var tradableAsset = tradableAssetManager.ActiveTradableAsset;

        if (tradableAsset == null)
            return;

        var assetBalance = "" + tradableAsset.AssetBalance;

        optionsDropdownButton.dropdownButtons = uiDropdowns.extraOptionsDropdowns
                                                           .Where(dropdown => dropdown.assetAddresses.ContainsIgnoreCase(tradableAsset.AssetAddress, true))
                                                           .Concat(uiDropdowns.optionsDropdowns)
                                                           .ToArray();

        assetText.text = StringUtils.LimitEnd(tradableAsset.AssetName, MAX_ASSET_NAME_LENGTH, "...");
        balanceText.text = StringUtils.LimitEnd(assetBalance, MAX_ASSET_BALANCE_LENGTH, "...");
        smallBalanceText.text = StringUtils.LimitEnd(assetBalance, MAX_SMALL_ASSET_BALANCE_LENGTH, "...");

        assetSymbolText.text = tradableAsset.AssetSymbol;

        assetImage.sprite = tradableAsset.AssetImage;
        smallAssetImage.sprite = tradableAsset.AssetImage;
    }

    public override void OnBackPressed()
    {
    }
}