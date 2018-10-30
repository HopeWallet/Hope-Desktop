using UnityEngine;

/// <summary>
/// Class that animates new buttons on the OpenWalletMenu.
/// </summary>
public class ButtonAnimator
{
    public ButtonAnimator(TradableAssetButtonManager tradableAssetButtonManager)
    {
        tradableAssetButtonManager.OnTradableAssetButtonCreated += asset => AnimateButtonIn(asset.transform.parent.gameObject);
        tradableAssetButtonManager.OnTradableAssetButtonRemoved += asset => AnimateButtonOut(asset.transform.parent.gameObject);
    }

	public void AnimateButtonIn(GameObject button) => button.AnimateScale(1f, 0.15f);

    public void AnimateButtonOut(GameObject button) => button.AnimateScale(0f, 0.15f, () => Object.Destroy(button));
}
