using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Class which manages the adding/enabling/disabling of tradable asset buttons.
/// </summary>
public class TradableAssetButtonManager
{

    private readonly Settings settings;
    private readonly TokenContractManager tokenContractManager;
    private readonly AssetButton.Factory buttonFactory;

    private readonly List<AssetButton> assetButtons = new List<AssetButton>();

    private AssetButton activeAssetButton;

    /// <summary>
    /// Initializes the TradableAssetButtonManager by injecting the settings and assigning all required methods to events.
    /// </summary>
    /// <param name="settings"> The settings to use for the button manager. </param>
    /// <param name="buttonFactory"> The factory which creates AssetButtons. </param>
    public TradableAssetButtonManager(Settings settings, AssetButton.Factory buttonFactory, TokenContractManager tokenContractManager) : base()
    {
        this.settings = settings;
        this.tokenContractManager = tokenContractManager;
        this.buttonFactory = buttonFactory;

        TradableAssetManager.OnBalancesUpdated += UpdateButtonBalances;
        TradableAssetManager.OnTradableAssetAdded += AddAssetButton;
        TradableAssetManager.OnTradableAssetRemoved += RemoveButton;

        TokenContractManager.OnTokensLoaded += SortButtons;
    }

    /// <summary>
    /// Adds a asset button to the list of asset buttons visible.
    /// </summary>
    /// <param name="tradableAsset"> The TokenContract which will be assigned to this button. </param>
    public void AddAssetButton(TradableAsset tradableAsset)
    {
        var assetButton = buttonFactory.Create().SetButtonInfo(tradableAsset);

        assetButtons.Add(assetButton);

        if (tradableAsset is EtherAsset)
            EnableNewTokenButton(assetButton);
    }

    /// <summary>
    /// Sorts the buttons in the order they were originally added.
    /// </summary>
    public void SortButtons()
    {
        assetButtons.Sort((b1, b2) => tokenContractManager.GetTokenIndex(b1.ButtonInfo.AssetAddress)
                                                                .CompareTo(tokenContractManager.GetTokenIndex(b2.ButtonInfo.AssetAddress)));
        assetButtons.ForEach(asset => asset.transform.SetSiblingIndex(settings.spawnTransform.childCount - 2));
    }

    /// <summary>
    /// Removes a TradableAsset's button from the list of buttons, and destroys the gameobject.
    /// </summary>
    /// <param name="assetToRemove"> The TradableAsset to remove. </param>
    public void RemoveButton(TradableAsset assetToRemove)
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
                Object.Destroy(assetButton.gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// Updates the balances on all the asset buttons.
    /// </summary>
    public void UpdateButtonBalances() => assetButtons.SafeForEach(button => button.UpdateButtonBalance());

    /// <summary>
    /// Enables a new asset button by making it interactable, and disabling the old one by making it non interactable.
    /// </summary>
    /// <param name="newAssetButton"> The new button to set as interactable. </param>
    public void EnableNewTokenButton(AssetButton newAssetButton)
    {
        if (activeAssetButton != null)
            activeAssetButton.Button.interactable = true;

        newAssetButton.Button.interactable = false;
        activeAssetButton = newAssetButton;
    }

    /// <summary>
    /// Class which contains settings for this TradableAssetButtonManager.
    /// </summary>
    [Serializable]
    public class Settings
    {
        public Transform spawnTransform;
    }
}
