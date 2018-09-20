/// <summary>
/// Class which manages the buttons for ERC20 token assets.
/// </summary>
public sealed class ERC20TokenAssetButton : TradableAssetButton<ERC20TokenAssetButton>
{
    /// <summary>
    /// The display text for the ERC20TokenAsset.
    /// </summary>
    protected override string AssetDisplayText => ButtonInfo.AssetSymbol.LimitEnd(5, "...");
}