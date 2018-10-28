using System;
using Zenject;

/// <summary>
/// Class used for displaying the popup for entering the trezor pin.
/// </summary>
public sealed class EnterTrezorPINPopup : ExitablePopupComponent<EnterTrezorPINPopup>
{
    public event Action ReloadPINSection;

    public event Action CheckingPIN;

    private TrezorWallet trezorWallet;

    private bool checkingPin;

    /// <summary>
    /// The section used for entering the trezor pin.
    /// </summary>
    public TrezorPINSection TrezorPINSection { get; private set; }

    /// <summary>
    /// Adds the TrezorWallet dependency.
    /// </summary>
    /// <param name="trezorWallet"> The active TrezorWallet. </param>
    [Inject]
    public void Construct(TrezorWallet trezorWallet)
    {
        this.trezorWallet = trezorWallet;
    }

    /// <summary>
    /// Called when the pin needs to be entered.
    /// Invokes the ReloadPINSection event.
    /// </summary>
    public void ReEnterPIN()
    {
        if (!checkingPin)
            return;

        MainThreadExecutor.QueueAction(() => ReloadPINSection?.Invoke());

        checkingPin = false;
    }

    /// <summary>
    /// Called when the pin starts being checked.
    /// Invokes the CheckingPIN event.
    /// </summary>
    public void CheckPIN()
    {
        MainThreadExecutor.QueueAction(() => CheckingPIN?.Invoke());

        checkingPin = true;
    }

    /// <summary>
    /// Initializes the TrezorPINSection.
    /// </summary>
    protected override void OnStart()
    {
        TrezorPINSection = GetComponentInChildren<TrezorPINSection>();
    }
}