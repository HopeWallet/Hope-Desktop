using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Class which manages the adding/enabling/disabling of tradable asset buttons.
/// </summary>
public sealed class TradableAssetButtonManager : IDisposable
{
    public event Action<ITradableAssetButton> OnActiveButtonChanged;

    private readonly TokenContractManager tokenContractManager;

    private readonly EtherAssetButton.Factory etherAssetButtonFactory;
    private readonly ERC20TokenAssetButton.Factory erc20TokenButtonFactory;

    private readonly List<ITradableAssetButton> assetButtons = new List<ITradableAssetButton>();

    private ITradableAssetButton activeAssetButton;

    /// <summary>
    /// Initializes the TradableAssetButtonManager by injecting the settings and assigning all required methods to events.
    /// </summary>
    /// <param name="erc20TokenButtonFactory"> The factory which creates ERC20TokenAssetButtons. </param>
    /// <param name="etherAssetButtonFactory"> The factory which creates EtherAssetButtons. </param>
    /// <param name="disposableComponentManager"> The active DisposableComponentManager. </param>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    public TradableAssetButtonManager(
        ERC20TokenAssetButton.Factory erc20TokenButtonFactory,
        EtherAssetButton.Factory etherAssetButtonFactory,
        DisposableComponentManager disposableComponentManager,
        TokenContractManager tokenContractManager,
        TradableAssetManager tradableAssetManager)
    {
        this.erc20TokenButtonFactory = erc20TokenButtonFactory;
        this.etherAssetButtonFactory = etherAssetButtonFactory;
        this.tokenContractManager = tokenContractManager;

        disposableComponentManager.AddDisposable(this);

        tradableAssetManager.OnTradableAssetAdded += AddAssetButton;
        tradableAssetManager.OnTradableAssetRemoved += RemoveButton;
        tokenContractManager.OnTokensLoaded += SortButtons;
    }

    /// <summary>
    /// Disposes of all created AssetButtons and clears the list.
    /// </summary>
    public void Dispose()
    {
        for (int i = 0; i < assetButtons.Count; i++)
            Object.Destroy(assetButtons[i].transform.parent.gameObject);

        assetButtons.Clear();
        activeAssetButton = null;
    }

    /// <summary>
    /// Resets the notifications for the list of AssetButtons.
    /// </summary>
    public void ResetButtonNotifications()
    {
        assetButtons.ForEach(asset => asset.ResetButtonNotifications());
    }

    /// <summary>
    /// Enables a new asset button by making it interactable, and disabling the old one by making it non interactable.
    /// </summary>
    /// <param name="newAssetButton"> The new button to set as interactable. </param>
    public void EnableNewTokenButton(ITradableAssetButton newAssetButton)
    {
        if (activeAssetButton != null)
            activeAssetButton.Button.interactable = true;

        newAssetButton.Button.interactable = false;
        activeAssetButton = newAssetButton;

        OnActiveButtonChanged?.Invoke(activeAssetButton);
    }

    /// <summary>
    /// Adds a asset button to the list of asset buttons visible.
    /// </summary>
    /// <param name="tradableAsset"> The TokenContract which will be assigned to this button. </param>
    private void AddAssetButton(TradableAsset tradableAsset)
    {
        ITradableAssetButton assetButton;

        if (tradableAsset is EtherAsset)
        {
            assetButton = etherAssetButtonFactory.Create().SetButtonInfo(tradableAsset);
            EnableNewTokenButton(assetButton);
        }
        else
        {
            assetButton = erc20TokenButtonFactory.Create().SetButtonInfo(tradableAsset);
        }

        assetButtons.Add(assetButton);

        OptimizedScrollview.GetScrollview("asset_scrollview")?.Refresh();
    }

    /// <summary>
    /// Removes a TradableAsset's button from the list of buttons, and destroys the gameobject.
    /// </summary>
    /// <param name="assetToRemove"> The TradableAsset to remove. </param>
    private void RemoveButton(TradableAsset assetToRemove)
    {
        for (int i = 0; i < assetButtons.Count; i++)
        {
            var assetButton = assetButtons[i];
            var asset = assetButton.ButtonInfo;

            if (asset == assetToRemove)
            {
                if (assetButton == activeAssetButton)
                    assetButtons[0].ButtonLeftClicked();

                assetButtons.Remove(assetButton);
                Object.Destroy(assetButton.transform.parent.gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// Sorts the buttons in the order they were originally added.
    /// </summary>
    private void SortButtons()
    {
        assetButtons.Sort((b1, b2) => tokenContractManager.GetTokenIndex(b1.ButtonInfo.AssetAddress)
                                                          .CompareTo(tokenContractManager.GetTokenIndex(b2.ButtonInfo.AssetAddress)));

        assetButtons.ForEach(asset => asset.transform.parent.SetAsLastSibling());
    }

    /// <summary>
    /// Class which contains settings for this TradableAssetButtonManager.
    /// </summary>
    [Serializable]
    public class Settings
    {
        public Transform spawnTransform;
        public Transform etherSpawnTransform;
    }
}