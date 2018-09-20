/// <summary>
/// Class which manages the button for the Ether asset button.
/// </summary>
public sealed class EtherAssetButton : TradableAssetButton<EtherAssetButton>
{
    /// <summary>
    /// The name/symbol display text for the EtherAsset.
    /// </summary>
    protected override string AssetDisplayText => "Ether (ETH)";

    /// <summary>
    /// The balance display text for the EtherAsset.
    /// </summary>
    protected override string AssetBalanceText => StringUtils.LimitEnd(ButtonInfo.AssetBalance.ToString(), 12, "...");
}