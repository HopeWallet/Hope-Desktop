using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class that is used for loading the images for certain assets.
/// </summary>
public sealed class TradableAssetImageManager
{
    private readonly Dictionary<string, Sprite> loadedImages = new Dictionary<string, Sprite>();
    private readonly Sprite defaultSprite;
    private readonly Vector2 pivot;

    private readonly CoinMarketCapDataManager coinMarketCapDataManager;

    /// <summary>
    /// Initializes the TradableAssetManager by getting the default sprite and initializing the dictionary.
    /// </summary>
    /// <param name="coinMarketCapDataManager"> Class which contains the list of CoinMarketCap coins with all the ids for image lookup. </param>
    public TradableAssetImageManager(CoinMarketCapDataManager coinMarketCapDataManager)
    {
        this.coinMarketCapDataManager = coinMarketCapDataManager;

        pivot = new Vector2(0.5f, 0.5f);
        defaultSprite = CreateSprite(Resources.Load("UI/Graphics/Textures/Icons/AssetLogos/DEFAULT") as Texture2D);
    }

    /// <summary>
    /// Gets the image for an asset given the asset's symbol.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset. </param>
    /// <param name="onImageReceived"> Action called once the image has been received. </param>
    public void LoadImage(string assetSymbol, Action<Sprite> onImageReceived)
    {
        if (LoadFromDictionary(assetSymbol, onImageReceived) || LoadFromResources(assetSymbol, onImageReceived))
            return;

        LoadFromWeb(assetSymbol, onImageReceived);
    }

    /// <summary>
    /// Loads the asset image from LiveCoinWatch if it exists.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset to load the image for. </param>
    /// <param name="onImageReceived"> Action to call once the image has been received. </param>
    private void LoadFromWeb(string assetSymbol, Action<Sprite> onImageReceived)
    {
        _DownloadImage(assetSymbol, image => SaveImageToDictionary(image, assetSymbol, onImageReceived)).StartCoroutine();
    }

    /// <summary>
    /// Loads the asset image from the resources folder.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset to load the image for. </param>
    /// <param name="onImageReceived"> Action to call once the image has been received. </param>
    /// <returns> Whether the image was found in the Resources folder or not. </returns>
    private bool LoadFromResources(string assetSymbol, Action<Sprite> onImageReceived)
    {
        Texture2D image;

        if ((image = Resources.Load("UI/Graphics/Textures/Icons/AssetLogos/" + assetSymbol) as Texture2D) == null)
            return false;

        SaveImageToDictionary(image, assetSymbol, onImageReceived);
        return true;
    }

    /// <summary>
    /// Loads the asset image from the dictionary if it has already been saved to it. 
    /// </summary>
    /// <param name="assetSymbol"></param>
    /// <param name="onImageReceived"></param>
    /// <returns></returns>
    private bool LoadFromDictionary(string assetSymbol, Action<Sprite> onImageReceived)
    {
        if (loadedImages.ContainsKey(assetSymbol))
        {
            onImageReceived?.Invoke(loadedImages[assetSymbol]);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Saves an asset image into the dictionary.
    /// </summary>
    /// <param name="image"> The image to save. </param>
    /// <param name="assetSymbol"> The symbol of the asset's image. </param>
    /// <param name="onImageReceived"> Action to call once the image has been saved. </param>
    private void SaveImageToDictionary(Texture2D image, string assetSymbol, Action<Sprite> onImageReceived)
    {
        if (!loadedImages.ContainsKey(assetSymbol))
            loadedImages.Add(assetSymbol, CreateSprite(image));

        onImageReceived?.Invoke(loadedImages[assetSymbol]);
    }

    /// <summary>
    /// Creates a sprite given a Texture2D.
    /// </summary>
    /// <param name="tex"> The texture to create the sprite from. </param>
    /// <returns> The newly created sprite. </returns>
    private Sprite CreateSprite(Texture2D tex)
    {
        if (tex == null)
            return defaultSprite;

        return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), pivot);
    }

    /// <summary>
    /// Coroutine which downloads an image from livecoinwatch images given the symbol of the asset.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset image to download. </param>
    /// <param name="onTextureReceived"> Action to call once the image has been downloaded. </param>
    /// <returns> Waits for the image download request to finish. </returns>
    private IEnumerator _DownloadImage(string assetSymbol, Action<Texture2D> onTextureReceived)
    {
        UnityWebRequest webRequest = null;
        int? assetId = coinMarketCapDataManager.GetCoinID(assetSymbol);

        if (assetId.HasValue)
        {
            webRequest = UnityWebRequestTexture.GetTexture("https://s2.coinmarketcap.com/static/img/coins/128x128/" + assetId.Value + ".png");
            yield return webRequest.SendWebRequest();
        }

        onTextureReceived?.Invoke(webRequest?.isNetworkError != false || webRequest.isHttpError ? null : DownloadHandlerTexture.GetContent(webRequest));
    }
}
