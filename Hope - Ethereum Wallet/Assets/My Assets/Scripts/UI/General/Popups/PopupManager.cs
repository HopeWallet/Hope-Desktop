﻿using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.Object;

/// <summary>
/// Class which manages the creation and destruction of all popups.
/// </summary>
public sealed class PopupManager
{
    private readonly List<object> factoryPopups = new List<object>();
    private readonly Stack<KeyValuePair<PopupBase, Action>> activePopups = new Stack<KeyValuePair<PopupBase, Action>>();

    /// <summary>
    /// Whether a popup is currently being animated.
    /// </summary>
    public bool AnimatingPopup { get; private set; }

    /// <summary>
    /// Whether an active popup currently exists.
    /// </summary>
    public bool ActivePopupExists => activePopups.Count > 0;

    /// <summary>
    /// The type of the active popup.
    /// </summary>
    public Type ActivePopupType => activePopups.Count == 0 ? null : activePopups.Peek().Key.GetType();

    /// <summary>
    /// Initializes the PopupManager by adding all required factories.
    /// </summary>
    public PopupManager(LoadingPopup.Factory loadingPopupFactory,
        AddTokenPopup.Factory addTokenPopupFactory,
        SendAssetPopup.Factory sendAssetPopupFactory,
        ConfirmTransactionPopup.Factory confirmSendAssetPopupFactory,
        ReceiveAssetPopup.Factory receiveAssetPopupFactory,
        TransactionInfoPopup.Factory transactionInfoPopupFactory,
        LockedPRPSPopup.Factory lockedPrpsPopupFactory,
        LockPRPSPopup.Factory lockPrpsPopupFactory,
        ConfirmLockPopup.Factory confirmPrpsLockPopupFactory,
        GeneralOkCancelPopup.Factory generalOkCancelPopupFactory,
        UnlockWalletPopup.Factory unlockWalletPopupFactory,
        ContactsPopup.Factory contactsPopupFactory,
        AddOrEditContactPopup.Factory addOrEditContactPopupFactory,
        TooltipPopup.Factory tooltipPopupFactory,
		ConnectionLostPopup.Factory conectionLostPopupFactory,
		SettingsPopup.Factory settingsPopupFactory,
		AccountsPopup.Factory accountsPopupFactory,
		AboutPopup.Factory aboutPopupFactory,
        ConfirmReleasePopup.Factory confirmReleasePopupFactory,
		TransactionWarningPopup.Factory transactionWarningPopupFactory,
		HopeUpdatePopup.Factory hopeUpdatePopupFactory,
        EnterTrezorPINPopup.Factory enterTrezorPinPopupFactory,
		HelpPopup.Factory helpPopupFactory,
		AssetInfoPopup.Factory assetInfoPopupFactory)
    {
        factoryPopups.AddItems(loadingPopupFactory,
							   addTokenPopupFactory,
							   sendAssetPopupFactory,
							   confirmSendAssetPopupFactory,
							   receiveAssetPopupFactory,
							   transactionInfoPopupFactory,
							   lockedPrpsPopupFactory,
							   lockPrpsPopupFactory,
							   confirmPrpsLockPopupFactory,
							   generalOkCancelPopupFactory,
							   unlockWalletPopupFactory,
							   contactsPopupFactory,
							   addOrEditContactPopupFactory,
							   tooltipPopupFactory,
							   conectionLostPopupFactory,
							   settingsPopupFactory,
							   accountsPopupFactory,
							   aboutPopupFactory,
                               confirmReleasePopupFactory,
							   transactionWarningPopupFactory,
							   hopeUpdatePopupFactory,
                               enterTrezorPinPopupFactory,
							   helpPopupFactory,
							   assetInfoPopupFactory);
    }

    /// <summary>
    /// Kills the active popup and ignores all safeguards to prevent popup closing.
    /// </summary>
    public void KillActivePopup()
    {
        if (activePopups.Count > 0)
            Destroy(activePopups.Pop().Key.gameObject);
    }

    /// <summary>
    /// Kills the active popup if it is of a certain type.
    /// </summary>
    /// <param name="popupTypeToKill"> The type of the popup to kill. </param>
    public void KillActivePopup(Type popupTypeToKill)
    {
        if (activePopups.Count > 0 && ActivePopupType == popupTypeToKill)
            Destroy(activePopups.Pop().Key.gameObject);
    }

    /// <summary>
    /// Closes the currently active popup if there are no popups animating currently and the active popup has closing enabled.
    /// </summary>
    /// <param name="typesToIgnore"> The filter for ignoring closing certain popups if the active popup is one of the types. </param>
    /// <returns> Whether the active popup was closed or not. </returns>
    public bool CloseActivePopup(params Type[] typesToIgnore)
    {
        if (activePopups.Count == 0)
            return false;

        PopupBase popup = activePopups.Peek().Key;

        if (popup.Animator?.Animating != false || popup.DisableClosing)
            return false;

        foreach (Type type in typesToIgnore)
            if (popup.GetType() == type)
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
    public TPopup GetPopup<TPopup>(bool stackPopups = true) where TPopup : FactoryPopup<TPopup>
    {
        var popupsOfType = activePopups.Where(popup => popup.Key.GetType() == typeof(TPopup));

        if (popupsOfType.Any())
            return popupsOfType.Single().Key as TPopup;
        else if (activePopups.Count > 0 && !stackPopups)
            return null;

        TPopup newPopup = factoryPopups.OfType<FactoryPopup<TPopup>.Factory>().Single().Create();
        activePopups.Push(new KeyValuePair<PopupBase, Action>(newPopup, () => AnimatePopup(newPopup, false, () => Destroy(newPopup.gameObject))));

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