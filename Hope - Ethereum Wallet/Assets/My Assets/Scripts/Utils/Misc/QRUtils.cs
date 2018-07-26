using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

/// <summary>
/// Class used to help with the creation and displaying of qr codes.
/// </summary>
public static class QRUtils
{

    private static readonly Dictionary<string, Texture2D> QrCodes = new Dictionary<string, Texture2D>();
    private static readonly Vector2 Pivot = new Vector2(0.5f, 0.5f);

    /// <summary>
    /// Generates a QR code for a given string and returns the sprite created from it.
    /// </summary>
    /// <param name="text"> The text to convert to a qr code representation. </param>
    /// <returns> The sprite of the qr code. </returns>
    public static Sprite GenerateQRCode(string text)
    {
        if (QrCodes.ContainsKey(text))
            return CreateSprite(QrCodes[text]);

        var encoded = new Texture2D(256, 256);
        var colors = Encode(text, encoded.width, encoded.height);

        ReplaceWhiteWithTransparency(colors);

        encoded.SetPixels32(colors);
        encoded.Apply();

        QrCodes.Add(text, encoded);

        return CreateSprite(encoded);
    }

    /// <summary>
    /// Replaces the qr code's white pixels with transparent pixels.
    /// </summary>
    /// <param name="colors"> The array of colors which belong to the qr code. </param>
    private static void ReplaceWhiteWithTransparency(Color32[] colors)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            var color = colors[i];

            if (color.r != 0 && color.g != 0 && color.b != 0)
                colors[i] = new Color32(255, 255, 255, 0);
        }
    }

    /// <summary>
    /// Converts a string to a qr code given the desired width and height.
    /// </summary>
    /// <param name="textForEncoding"> The text to encode to qr code format. </param>
    /// <param name="width"> The width of the qr code. </param>
    /// <param name="height"> The height of the qr code. </param>
    /// <returns> The array of colors of the qr code. </returns>
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };

        return writer.Write(textForEncoding);
    }

    /// <summary>
    /// Creates a sprite from a Texture2D.
    /// </summary>
    /// <param name="texture"> The texture to create a sprite from. </param>
    /// <returns> The sprite created from the texture. </returns>
    private static Sprite CreateSprite(Texture2D texture) => Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Pivot);
}
