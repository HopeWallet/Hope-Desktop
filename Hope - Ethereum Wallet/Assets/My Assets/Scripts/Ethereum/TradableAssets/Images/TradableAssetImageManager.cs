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

    private readonly string imagePath;

    /// <summary>
    /// Initializes the TradableAssetManager by getting the default sprite and initializing the dictionary.
    /// </summary>
    public TradableAssetImageManager()
    {
        imagePath = Application.streamingAssetsPath + "/";
        pivot = new Vector2(0.5f, 0.5f);
        defaultSprite = CreateSprite(Resources.Load("UI/Graphics/Icons/AssetLogos/DEFAULT") as Texture2D);
    }

    /// <summary>
    /// Gets the image for an asset given the asset's symbol.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset. </param>
    /// <param name="onImageReceived"> Action called once the image has been received. </param>
    public void LoadImage(string assetSymbol, Action<Sprite> onImageReceived = null)
    {
        if (LoadFromDictionary(assetSymbol, onImageReceived) || LoadFromResources(assetSymbol, onImageReceived))
            return;

        LoadFromFiles(assetSymbol, image =>
        {
            if (image == null)
                LoadFromWeb(assetSymbol, onImageReceived);
            else
                onImageReceived?.Invoke(image);
        });
    }

    /// <summary>
    /// Loads the asset image from LiveCoinWatch if it exists.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset to load the image for. </param>
    /// <param name="onImageReceived"> Action to call once the image has been received. </param>
    private void LoadFromWeb(string assetSymbol, Action<Sprite> onImageReceived)
    {
        _DownloadImage(assetSymbol, image =>
        {
            if (image != null)
                SaveImageFile(image, assetSymbol);

            SaveImageToDictionary(image, assetSymbol, onImageReceived);
        }).StartCoroutine();
    }

    /// <summary>
    /// Loads the asset image from the Resources folder if it exists.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset to load the image for. </param>
    /// <param name="onImageReceived"> Action to call once the image has been received. </param>
    private void LoadFromFiles(string assetSymbol, Action<Sprite> onImageReceived)
    {
        if (!File.Exists(GetFilePath(assetSymbol)))
        {
            onImageReceived?.Invoke(null);
            return;
        }

        _LoadFromStreamingAssets(assetSymbol, image =>
        {
            if (image != null)
                SaveImageToDictionary(image, assetSymbol, onImageReceived);
            else
                onImageReceived?.Invoke(null);
        }).StartCoroutine();
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

        if ((image = Resources.Load("UI/Graphics/Icons/AssetLogos/" + assetSymbol) as Texture2D) == null)
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
    /// Gets the file path for an asset given its symbol.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset to get the file path for. </param>
    /// <returns> The file path for the asset's image. </returns>
    private string GetFilePath(string assetSymbol) => imagePath + assetSymbol.ToUpper() + ".png";

    /// <summary>
    /// Saves an image to the streaming assets folder.
    /// </summary>
    /// <param name="image"> The image to save. </param>
    /// <param name="assetSymbol"> The symbol of the asset which will be saved. </param>
    private void SaveImageFile(Texture2D image, string assetSymbol) => File.WriteAllBytes(GetFilePath(assetSymbol), image.EncodeToPNG());

    /// <summary>
    /// Coroutine which downloads an image from livecoinwatch images given the symbol of the asset.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset image to download. </param>
    /// <param name="onTextureReceived"> Action to call once the image has been downloaded. </param>
    /// <returns> Waits for the image download request to finish. </returns>
    private IEnumerator _DownloadImage(string assetSymbol, Action<Texture2D> onTextureReceived)
	{ 
        var request = UnityWebRequestTexture.GetTexture("https://www.livecoinwatch.com/images/icons64/" + assetSymbol.ToLower() + ".png");
        yield return request.SendWebRequest();

        onTextureReceived?.Invoke(request.isNetworkError || request.isHttpError ? null : DownloadHandlerTexture.GetContent(request));
    }

    /// <summary>
    /// Coroutine which loads an image from the streaming assets folder.
    /// </summary>
    /// <param name="assetSymbol"> The symbol of the asset to load from streaming assets. </param>
    /// <param name="onTextureReceived"> Action to call once the image has been loaded from the streaming assets. </param>
    /// <returns> Waits for the image to be loaded from the streaming assets. </returns>
    private IEnumerator _LoadFromStreamingAssets(string assetSymbol, Action<Texture2D> onTextureReceived)
    {
        var www = new WWW("file://" + GetFilePath(assetSymbol));
        yield return www;

        onTextureReceived?.Invoke(string.IsNullOrEmpty(www.error) ? null : www.texture);
    }
}
