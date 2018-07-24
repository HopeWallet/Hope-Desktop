using System;
using TMPro;
using UnityEngine.UI;

public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
    public sealed class AssetManager : IUpdater, IEtherBalanceObservable
    {
        private readonly TradableAssetManager tradableAssetManager;
        private readonly TradableAssetImageManager tradableAssetImageManager;
        private readonly EtherBalanceObserver etherBalanceObserver;
        private readonly UpdateManager updateManager;

        private readonly TMP_Text assetBalance,
                                  assetSymbol;

        private readonly Image assetImage;

        private dynamic lastEtherBalance,
                        lastAssetBalance;

        private Action onAssetBalanceChanged;

        public dynamic EtherBalance { get; set; }

        public dynamic ActiveAssetBalance => ActiveAsset.AssetBalance;

        public TradableAsset ActiveAsset => tradableAssetManager.ActiveTradableAsset;

        public AssetManager(
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

        public void AddAssetBalanceListener(Action onBalanceChanged)
        {
            if (onAssetBalanceChanged == null)
                onAssetBalanceChanged = onBalanceChanged;
            else
                onAssetBalanceChanged += onBalanceChanged;
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

        private void UpdateBalance()
        {
            var newAssetBalance = tradableAssetManager.ActiveTradableAsset.AssetBalance;
            var newEtherBalance = EtherBalance;

            assetBalance.text = StringUtils.LimitEnd(tradableAssetManager.ActiveTradableAsset.AssetBalance.ToString(), 14, "...");

            if (lastAssetBalance != newAssetBalance || lastEtherBalance != newEtherBalance)
                onAssetBalanceChanged?.Invoke();

            lastAssetBalance = newAssetBalance;
            lastEtherBalance = newEtherBalance;
        }

        private void UpdateSymbol() => assetSymbol.text = tradableAssetManager.ActiveTradableAsset.AssetSymbol;

        private void UpdateImage() => tradableAssetImageManager.LoadImage(tradableAssetManager.ActiveTradableAsset.AssetSymbol, img => assetImage.sprite = img);

    }
}