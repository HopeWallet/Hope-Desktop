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

    public bool AnimatingPopup { get; private set; }

    /// <summary>
    /// Initializes the PopupManager by adding all required factories.
    /// </summary>
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
        GeneralTransactionConfirmationPopup.Factory generalConfirmationPopupFactory,
        UnlockWalletPopup.Factory unlockWalletPopupFactory)
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
                          generalConfirmationPopupFactory,
                          unlockWalletPopupFactory);
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
    /// Closes all active popups.
    /// </summary>
    public void CloseAllPopups()
    {
        while (activePopups.Count > 0)
            activePopups.Pop().Value?.Invoke();
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

        TPopup newPopup = factoryPopups.OfType<FactoryPopup<TPopup>.Factory>().Single().Create();
        activePopups.Push(new KeyValuePair<object, Action>(newPopup, () => AnimatePopup(newPopup, false, () => Object.Destroy(newPopup.gameObject))));

        AnimatePopup(newPopup, true, null);

        return newPopup;
    }

    /// <summary>
    /// Animates a popup if it has a <see cref="UIAnimator"/> component.
    /// </summary>
    /// <typeparam name="TPopup"> The type of the popup to animate. </typeparam>
    /// <param name="newPopup"> The instance of the popup to animate. </param>
    /// <param name="animateEnable"> Whether the popup should be animated for enable or disable. </param>
    /// <param name="onAnimationFinished"> Action called once the animation is finished. </param>
    private void AnimatePopup<TPopup>(TPopup newPopup, bool animateEnable, Action onAnimationFinished) where TPopup : FactoryPopup<TPopup>
    {
        var popupAnimator = newPopup.Animator;

        if (popupAnimator != null)
        {
            AnimatingPopup = true;

            Action<Action> animateAction;
            if (animateEnable)
                animateAction = popupAnimator.AnimateEnable;
            else
                animateAction = popupAnimator.AnimateDisable;

            animateAction?.Invoke(() =>
            {
                AnimatingPopup = false;
                onAnimationFinished?.Invoke();
            });
        }
        else
        {
            onAnimationFinished?.Invoke();
        }
    }
}
