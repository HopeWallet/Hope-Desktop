/// <summary>
/// Class which manages the buttons for ERC20 token assets.
/// </summary>
public sealed class ERC20TokenAssetButton : TradableAssetButton<ERC20TokenAssetButton>
{
    /// <summary>
    /// The symbol display text for the ERC20TokenAsset.
    /// </summary>
    protected override string AssetDisplayText => ButtonInfo.AssetSymbol.LimitEnd(5, "...");

    /// <summary>
    /// The balance display text for the ERC20TokenAsset.
    /// </summary>
    protected override string AssetBalanceText => StringUtils.LimitEnd(ButtonInfo.AssetBalance.ToString(), 7, "...");
}