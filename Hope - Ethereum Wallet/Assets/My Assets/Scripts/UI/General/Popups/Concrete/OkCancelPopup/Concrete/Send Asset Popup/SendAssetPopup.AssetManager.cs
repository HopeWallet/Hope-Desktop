using System;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class which displays the popup for sending a TradableAsset.
/// </summary>
public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
    /// <summary>
    /// Class which manages the asset that will be sent.
    /// </summary>
    public sealed class AssetManager : IUpdater, IEtherBalanceObservable
    {
        public event Action OnAssetBalanceChanged;

        private readonly TradableAssetManager tradableAssetManager;
        private readonly TradableAssetImageManager tradableAssetImageManager;
        private readonly EtherBalanceObserver etherBalanceObserver;
        private readonly UpdateManager updateManager;

        private readonly TMP_Text assetBalance,
                                  assetSymbol;

        private readonly Image assetImage;

        private dynamic lastEtherBalance,
                        lastAssetBalance;

        /// <summary>
        /// The current ether balance of this wallet.
        /// </summary>
        public dynamic EtherBalance { get; set; }

        /// <summary>
        /// The current balance of the active asset.
        /// </summary>
        public dynamic ActiveAssetBalance => ActiveAsset.AssetBalance;

        /// <summary>
        /// The active TradableAsset.
        /// </summary>
        public TradableAsset ActiveAsset => tradableAssetManager.ActiveTradableAsset;

        /// <summary>
        /// Initializes the <see cref="AssetManager"/> by assigning all required references.
        /// </summary>
        /// <param name="tradableAssetManager"> The active <see cref="TradableAssetManager"/>. </param>
        /// <param name="tradableAssetImageManager"> The active <see cref="TradableAssetImageManager"/>. </param>
        /// <param name="etherBalanceObserver"> The active <see cref="EtherBalanceObserver"/>. </param>
        /// <param name="updateManager"> The active <see cref="UpdateManager"/>. </param>
        /// <param name="assetSymbol"> The asset symbol text component. </param>
        /// <param name="assetBalance"> The asset balance text component. </param>
        /// <param name="assetImage"> The image component used for assigning the asset image. </param>
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

        /// <summary>
        /// Gets the current balance of the active tradable asset.
        /// </summary>
        public void UpdaterUpdate()
        {
            UpdateBalance();
        }

        /// <summary>
        /// Removes the observable and updater for updating the balances of the active asset.
        /// </summary>
        public void Destroy()
        {
            updateManager.RemoveUpdater(this);
            etherBalanceObserver.UnsubscribeObservable(this);
        }

        /// <summary>
        /// Starts the updaters and sets the initial data of the active tradable asset.
        /// </summary>
        private void Start()
        {
            StartUpdaters();
            UpdateBalance();
            UpdateSymbol();
            UpdateImage();
        }

        /// <summary>
        /// Starts the updater and observable.
        /// </summary>
        private void StartUpdaters()
        {
            updateManager.AddUpdater(this);
            etherBalanceObserver.SubscribeObservable(this);
        }

        /// <summary>
        /// Updates the balance of the active asset and invokes the onAssetBalanceChanged action if the active asset or ether balance changed.
        /// </summary>
        private void UpdateBalance()
        {
            var newAssetBalance = tradableAssetManager.ActiveTradableAsset.AssetBalance;
            var newEtherBalance = EtherBalance;

            assetBalance.text = StringUtils.LimitEnd(tradableAssetManager.ActiveTradableAsset.AssetBalance.ToString(), 14, "...");

            if (lastAssetBalance != newAssetBalance || lastEtherBalance != newEtherBalance)
                OnAssetBalanceChanged?.Invoke();

            lastAssetBalance = newAssetBalance;
            lastEtherBalance = newEtherBalance;
        }

        /// <summary>
        /// Updates the symbol of the active tradable asset.
        /// </summary>
        private void UpdateSymbol() => assetSymbol.text = tradableAssetManager.ActiveTradableAsset.AssetSymbol;

        /// <summary>
        /// Updates the image of the active tradable asset.
        /// </summary>
        private void UpdateImage() => tradableAssetImageManager.LoadImage(tradableAssetManager.ActiveTradableAsset.AssetSymbol, img => assetImage.sprite = img);
    }
}