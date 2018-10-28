using System;
using System.Threading.Tasks;
using Trezor.Net;

/// <summary>
/// Class used for displaying the menu for opening the trezor wallet.
/// </summary>
public sealed class OpenTrezorWalletMenu : OpenHardwareWalletMenu<OpenTrezorWalletMenu, TrezorWallet>
{
    public event Action TrezorPINSectionOpening;
    public event Action ReloadPINSection;

    public event Action CheckingPIN;

    private bool pinSectionOpen;

    /// <summary>
    /// The section used for entering the pin into the Trezor.
    /// </summary>
    public TrezorPINSection TrezorPINSection { get; private set; }

    /// <summary>
    /// Initializes the TrezorPINSection.
    /// </summary>
    protected override void OnAwake()
    {
        TrezorPINSection = GetComponentInChildren<TrezorPINSection>();
    }

    /// <summary>
    /// Adds the required pin callback.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        hardwareWallet.PINIncorrect += PINIncorrect;
    }

    /// <summary>
    /// Removes the pin callback.
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        hardwareWallet.PINIncorrect -= PINIncorrect;
    }

    /// <summary>
    /// Calls events based on the current status of the pin section.
    /// </summary>
    public void UpdatePINSection()
    {
        if (!pinSectionOpen)
        {
            pinSectionOpen = true;

            MainThreadExecutor.QueueAction(() => TrezorPINSectionOpening?.Invoke());
        }
        else
        {
            MainThreadExecutor.QueueAction(() => ReloadPINSection?.Invoke());
        }
    }

    /// <summary>
    /// Calls the CheckingPIN event.
    /// </summary>
    public void CheckPIN()
    {
        MainThreadExecutor.QueueAction(() => CheckingPIN?.Invoke());
    }

    /// <summary>
    /// Checks if the trezor is connected.
    /// </summary>
    /// <returns> Task returning the status of the trezor connection. </returns>
    protected override async Task<bool> IsHardwareWalletConnected()
    {
        var trezorManager = await Task<TrezorManager>.Factory.StartNew(() => TrezorConnector.GetWindowsConnectedTrezor(null)).ConfigureAwait(false);

        return trezorManager != null;
    }

    /// <summary>
    /// Called when the pin is entered incorrectly, setting the input field to an error.
    /// </summary>
    private void PINIncorrect()
    {
        TrezorPINSection.PINInputField.Error = true;
		TrezorPINSection.PINInputField.UpdateVisuals();
	}
}