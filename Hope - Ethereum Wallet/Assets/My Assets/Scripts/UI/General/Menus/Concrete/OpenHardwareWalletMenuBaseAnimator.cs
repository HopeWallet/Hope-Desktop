using UnityEngine;

public abstract class OpenHardwareWalletMenuBaseAnimator<THardwareWalletMenu, THardwareWallet> : MenuAnimator

    where THardwareWalletMenu : OpenHardwareWalletMenu<THardwareWalletMenu, THardwareWallet>

    where THardwareWallet : HardwareWallet
{
    [SerializeField] protected GameObject loadingWalletText;
    [SerializeField] protected GameObject awaitingConnectionText;
    [SerializeField] protected GameObject deviceConnectedText;
    [SerializeField] protected GameObject loadingIcon;
    [SerializeField] protected GameObject openWalletButton;

    private void Awake()
    {
        var walletMenuClass = transform.GetComponent<THardwareWalletMenu>();

        walletMenuClass.OnHardwareWalletConnected += () => ChangeWalletStatus(true);
        walletMenuClass.OnHardwareWalletDisconnected += () => ChangeWalletStatus(false);
        walletMenuClass.OnHardwareWalletLoadStart += () => ChangeLoadStatus(true);
        walletMenuClass.OnHardwareWalletLoadEnd += () => ChangeLoadStatus(false);
    }

    /// <summary>
    /// Changes the loading status of the hardware wallet
    /// </summary>
    /// <param name="loadingWallet"> Whether the wallet is loading or not </param>
    protected void ChangeLoadStatus(bool loadingWallet)
    {
        Animating = loadingWallet;
        SwitchObjects(loadingWallet ? deviceConnectedText : loadingWalletText, loadingWallet ? loadingWalletText : awaitingConnectionText);
        SwitchObjects(loadingWallet ? openWalletButton : loadingIcon, loadingWallet ? loadingIcon : loadingIcon);
    }

    /// <summary>
    /// Switches the button and text according to the status of the Ledger
    /// </summary>
    /// <param name="ledgerConnected"></param>
    protected void ChangeWalletStatus(bool ledgerConnected)
    {
        SwitchObjects(ledgerConnected ? awaitingConnectionText : deviceConnectedText, ledgerConnected ? deviceConnectedText : awaitingConnectionText);
        SwitchObjects(ledgerConnected ? loadingIcon : openWalletButton, ledgerConnected ? openWalletButton : loadingIcon);
    }

    /// <summary>
    /// Switches one object with another
    /// </summary>
    /// <param name="gameObjectOut"> The object being animated out </param>
    /// <param name="gameObjectIn"> The object being animated in </param>
    protected void SwitchObjects(GameObject gameObjectOut, GameObject gameObjectIn)
    {
        if (gameObjectOut == null)
            return;

        gameObjectOut.AnimateGraphicAndScale(0f, 0f, 0.15f, () =>
        {
            if (gameObjectIn != null)
                gameObjectIn.AnimateGraphicAndScale(1f, 1f, 0.15f);
        });
    }
}
