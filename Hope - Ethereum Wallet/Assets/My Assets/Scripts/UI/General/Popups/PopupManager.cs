using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

/// <summary>
/// Class which manages the creation and destruction of all popups.
/// </summary>
public class PopupManager
{

    private readonly List<object> factoryPopups = new List<object>();

    private Action closePopup;
    private object activePopup;

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
    /// <param name="prpsHodlPopupFactory"> The factory for creating PRPSHodlPopups. </param>
    public PopupManager(LoadingPopup.Factory loadingPopupFactory,
        AddTokenPopup.Factory addTokenPopupFactory,
        SendAssetPopup.Factory sendAssetPopupFactory,
        ConfirmSendAssetPopup.Factory confirmSendAssetPopupFactory,
        HideAssetPopup.Factory hideAssetPopupFactory,
        ConfirmHideAssetPopup.Factory confirmHideAssetPopupFactory,
        ReceiveAssetPopup.Factory receiveAssetPopupFactory,
        TransactionInfoPopup.Factory transactionInfoPopupFactory,
        PRPSHodlPopup.Factory prpsHodlPopupFactory) : base()
    {
        factoryPopups.AddItems(loadingPopupFactory, 
                          addTokenPopupFactory, 
                          sendAssetPopupFactory, 
                          confirmSendAssetPopupFactory,
                          hideAssetPopupFactory,
                          confirmHideAssetPopupFactory,
                          receiveAssetPopupFactory,
                          transactionInfoPopupFactory,
                          prpsHodlPopupFactory);
    }

    /// <summary>
    /// Closes the currently active popup.
    /// </summary>
    /// <param name="typesToIgnore"> The filter for ignoring closing certain popups if the active popup is one of the types. </param>
    /// <returns> Whether the active popup was closed or not. </returns>
    public bool CloseActivePopup(params Type[] typesToIgnore)
    {
        if (activePopup == null)
            return false;

        foreach (Type type in typesToIgnore)
            if (activePopup.GetType() == type)
                return false;

        activePopup = null;
        closePopup?.Invoke();

        return true;
    }

    /// <summary>
    /// Gets the current active popup or creates a new popup given the popup type.
    /// </summary>
    /// <typeparam name="TPopup"> The type of the popup. </typeparam>
    /// <returns> The popup created or retrieved. </returns>
    public TPopup GetPopup<TPopup>() where TPopup : FactoryPopup<TPopup>
    {
        if (activePopup != null)
            return activePopup as TPopup;

        var popup = factoryPopups.OfType<FactoryPopup<TPopup>.Factory>().First().Create();
        closePopup = () => Object.Destroy(popup.gameObject);
        activePopup = popup;

        return popup;
    }
}
