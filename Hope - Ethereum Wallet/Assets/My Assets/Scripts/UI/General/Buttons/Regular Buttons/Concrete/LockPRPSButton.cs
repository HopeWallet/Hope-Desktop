using UnityEngine;
using Zenject;

/// <summary>
/// Class used for the Lock Purpose button in the main OpenWalletMenu.
/// </summary>
public sealed class LockPRPSButton : ButtonBase
{
    private PopupManager popupManager;
    private LockedPRPSManager lockedPRPSManager;

    /// <summary>
    /// Adds the dependencies to the LockPRPSButton.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="lockedPRPSManager"> The active LockedPRPSManager. </param>
    [Inject]
    public void Construct(PopupManager popupManager, LockedPRPSManager lockedPRPSManager)
    {
        this.popupManager = popupManager;
        this.lockedPRPSManager = lockedPRPSManager;
    }

    /// <summary>
    /// Opens the LockPRPSPopup if no purpose has been locked yet, otherwise opens the LockedPRPSPopup.
    /// </summary>
    public override void ButtonLeftClicked()
    {
		if (lockedPRPSManager.UnfulfilledItems?.Count > 0)
            popupManager.GetPopup<LockedPRPSPopup>();
        else
            popupManager.GetPopup<LockPRPSPopup>();
    }
}