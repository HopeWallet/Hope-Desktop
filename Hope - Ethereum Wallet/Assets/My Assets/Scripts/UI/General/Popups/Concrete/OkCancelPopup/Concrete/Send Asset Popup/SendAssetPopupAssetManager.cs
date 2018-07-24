using TMPro;
using UnityEngine.UI;

public sealed class SendAssetPopupAssetManager : IUpdater, IEtherBalanceObservable
{
    private readonly TradableAssetManager tradableAssetManager;
    private readonly TradableAssetImageManager tradableAssetImageManager;
    private readonly EtherBalanceObserver etherBalanceObserver;
    private readonly UpdateManager updateManager;

    private readonly TMP_Text assetBalance,
                              assetSymbol;

    private readonly Image assetImage;

    public dynamic EtherBalance { get; set; }

    public dynamic ActiveAssetBalance { get { return tradableAssetManager.ActiveTradableAsset.AssetBalance; } }

    public TradableAsset ActiveAsset { get { return tradableAssetManager.ActiveTradableAsset; } }

    public SendAssetPopupAssetManager(
        TradableAssetManager tradableAssetManager,
        TradableAssetImageManager tradableAssetImageManager,
        EtherBalanceObserver etherBalanceObserver,
        UpdateManager updateManager,
        TMP_Text assetSymbol,
        TMP_Text assetBalance,
        Image assetImage)
    {
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.etherBalanceObserver = etherBalanceObserver;
        this.updateManager = updateManager;
        this.assetSymbol = assetSymbol;
        this.assetBalance = assetBalance;
        this.assetImage = assetImage;

        Start();
    }

    public void UpdaterUpdate()
    {
        UpdateBalance();
    }

    public void Destroy()
    {
        updateManager.RemoveUpdater(this);
        etherBalanceObserver.UnsubscribeObservable(this);
    }

    private void Start()
    {
        StartUpdaters();
        UpdateBalance();
        UpdateSymbol();
        UpdateImage();
    }

    private void StartUpdaters()
    {
        updateManager.AddUpdater(this);
        etherBalanceObserver.SubscribeObservable(this);
    }

    private void UpdateBalance() => assetBalance.text = StringUtils.LimitEnd(tradableAssetManager.ActiveTradableAsset.AssetBalance.ToString(), 14, "...");

    private void UpdateSymbol() => assetSymbol.text = tradableAssetManager.ActiveTradableAsset.AssetSymbol;

    private void UpdateImage() => tradableAssetImageManager.LoadImage(tradableAssetManager.ActiveTradableAsset.AssetSymbol, img => assetImage.sprite = img);

}