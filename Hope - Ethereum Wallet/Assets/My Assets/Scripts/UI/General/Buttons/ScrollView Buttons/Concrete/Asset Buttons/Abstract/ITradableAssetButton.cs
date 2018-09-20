using UnityEngine;
using UnityEngine.UI;

public interface ITradableAssetButton
{
    TradableAsset ButtonInfo { get; }

    Transform transform { get; }

    Button Button { get; }

    void ResetButtonNotifications();

    void ButtonLeftClicked();
}