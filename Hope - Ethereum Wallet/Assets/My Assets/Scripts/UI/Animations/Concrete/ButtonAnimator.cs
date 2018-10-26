using UnityEngine;

/// <summary>
/// Class that animates new buttons on the OpenWalletMenu
/// </summary>
public class ButtonAnimator
{

    public ButtonAnimator(TradableAssetButtonManager tradableAssetButtonManager)
    {
        // subscribe to events
    }

	public void AnimateNewButton(GameObject button) => button.AnimateScale(1f, 0.15f);
}
