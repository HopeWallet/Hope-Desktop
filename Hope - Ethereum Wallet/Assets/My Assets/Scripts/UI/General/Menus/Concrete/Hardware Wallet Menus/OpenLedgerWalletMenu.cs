using Ledger.Net.Connectivity;
using Nethereum.HdWallet;
using System;

/// <summary>
/// Class used for displaying the menu for opening the ledger wallet.
/// </summary>
public sealed class OpenLedgerWalletMenu : OpenHardwareWalletMenu<OpenLedgerWalletMenu, LedgerWallet>
{
    public override event Action OnHardwareWalletConnected;
    public override event Action OnHardwareWalletDisconnected;

    private bool connected;

    /// <summary>
    /// Checks if the ledger is connected and executes the OnLedgerConnected and OnLedgerDisconnected events accordingly.
    /// </summary>
    public override async void PeriodicUpdate()
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();
        var address = ledgerManager == null
            ? null
            : (await ledgerManager.GetPublicKeyResponse(Wallet.ELECTRUM_LEDGER_PATH.Replace("x", "0"), false, false).ConfigureAwait(false))?.Address;

        if (string.IsNullOrEmpty(address))
        {
            if (connected)
                MainThreadExecutor.QueueAction(() => OnHardwareWalletDisconnected?.Invoke());

            connected = false;
        }
        else
        {
            if (!connected)
                MainThreadExecutor.QueueAction(() => OnHardwareWalletConnected?.Invoke());

            connected = true;
        }
    }
}