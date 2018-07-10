using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

/// <summary>
/// Class which manages the creation and destruction of all popups.
/// </summary>
public sealed class PopupManager
{

    private readonly List<object> factoryPopups = new List<object>();
    private readonly Stack<KeyValuePair<object, Action>> activePopups = new Stack<KeyValuePair<object, Action>>();

    /// <summary>
    /// Initializes the PopupManager by adding all required factories.
    /// </summary>
    /// <param name="loadingPopupFactory"> The factory for creating LoadingPopups. </param>
    /// <param name="addTokenPopupFactory"> The factory for creating AddTokenPopups. </param>
    /// <param name="sendAssetPopupFactory"> The factory for creating SendAssetPopups. </param>
    /// <param name="confirmSendAssetPopupFactory"> The factory for creating ConfirmSendAssetPopups. </param>
    /// <param name="hideAssetPopupFactory"> The factory for creating HideAssetPopups. </param>
    /// <param name="confirmHideAssetPopupFactory"> The factory for creating ConfirmHideAssetPopups. </param>
    /// <param name="receiveAssetPopupFactory"> The factory for creating ReceiveAssetPopups. </param>
    /// <param name="transactionInfoPopupFactory"> The factory for creating TransactionInfoPopups. </param>
    /// <param name="prpsLockPopupFactory"> The factory for creating PRPSLockPopups. </param>
    /// <param name="confirmPrpsLockPopupFactory"> The factory for creating ConfirmPRPSLockPopups. </param>
    /// <param name="generalConfirmationPopupFactory"> The factory for creating GeneralTransactionConfirmationPopups. </param>
    public PopupManager(LoadingPopup.Factory loadingPopupFactory,
        AddTokenPopup.Factory addTokenPopupFactory,
        SendAssetPopup.Factory sendAssetPopupFactory,
        ConfirmSendAssetPopup.Factory confirmSendAssetPopupFactory,
        HideAssetPopup.Factory hideAssetPopupFactory,
        ConfirmHideAssetPopup.Factory confirmHideAssetPopupFactory,
        ReceiveAssetPopup.Factory receiveAssetPopupFactory,
        TransactionInfoPopup.Factory transactionInfoPopupFactory,
        PRPSLockPopup.Factory prpsLockPopupFactory,
        ConfirmPRPSLockPopup.Factory confirmPrpsLockPopupFactory,
        GeneralTransactionConfirmationPopup.Factory generalConfirmationPopupFactory) : base()
    {
        factoryPopups.AddItems(loadingPopupFactory,
                          addTokenPopupFactory,
                          sendAssetPopupFactory,
                          confirmSendAssetPopupFactory,
                          hideAssetPopupFactory,
                          confirmHideAssetPopupFactory,
                          receiveAssetPopupFactory,
                          transactionInfoPopupFactory,
                          prpsLockPopupFactory,
                          confirmPrpsLockPopupFactory,
                          generalConfirmationPopupFactory);
    }

    /// <summary>
    /// Closes the currently active popup.
    /// </summary>
    /// <param name="typesToIgnore"> The filter for ignoring closing certain popups if the active popup is one of the types. </param>
    /// <returns> Whether the active popup was closed or not. </returns>
    public bool CloseActivePopup(params Type[] typesToIgnore)
    {
        if (activePopups.Count == 0)
            return false;

        foreach (Type type in typesToIgnore)
            if (activePopups.Peek().Key.GetType() == type)
                return false;

        activePopups.Pop().Value?.Invoke();

        return true;
    }

    /// <summary>
    /// Gets the current active popup or creates a new popup given the popup type.
    /// </summary>
    /// <typeparam name="TPopup"> The type of the popup. </typeparam>
    /// <param name="stackPopups"> Whether this popup should stack on top of the last popup. </param>
    /// <returns> The popup created or retrieved. </returns>
    public TPopup GetPopup<TPopup>(bool stackPopups = false) where TPopup : FactoryPopup<TPopup>
    {
        var popupsOfType = activePopups.Where(popup => popup.Key.GetType() == typeof(TPopup));
        if (popupsOfType.Any())
            return popupsOfType.Single().Key as TPopup;
        else if (activePopups.Count > 0 && !stackPopups)
            return null;

        var newPopup = factoryPopups.OfType<FactoryPopup<TPopup>.Factory>().First().Create();
        activePopups.Push(new KeyValuePair<object, Action>(newPopup, () => Object.Destroy(newPopup.gameObject)));

        return newPopup;
    }
}
