using Ledger.Net;
using Ledger.Net.Connectivity;
using Nethereum.HdWallet;
using System.Threading.Tasks;

/// <summary>
/// Class used for displaying the menu for opening the ledger wallet.
/// </summary>
public sealed class OpenLedgerWalletMenu : OpenHardwareWalletMenu<OpenLedgerWalletMenu, LedgerWallet>
{
    /// <summary>
    /// Checks if the ledger is currently connected and on the Ethereum application.
    /// </summary>
    /// <returns> Task returning the current connection status. </returns>
    protected override async Task<bool> IsHardwareWalletConnected()
    {
        var ledgerManager = await Task<LedgerManager>.Factory.StartNew(LedgerConnector.GetWindowsConnectedLedger).ConfigureAwait(false);
        var address = ledgerManager == null
            ? null
            : (await ledgerManager.GetPublicKeyResponse(Wallet.ELECTRUM_LEDGER_PATH.Replace("x", "0"), false, false).ConfigureAwait(false))?.Address;

        return !string.IsNullOrEmpty(address);
    }
}