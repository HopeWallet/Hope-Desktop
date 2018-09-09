using Ledger.Net.Connectivity;
using System;
using Zenject;

public sealed class OpenLedgerWalletMenu : Menu<OpenLedgerWalletMenu>, IPeriodicUpdater
{
    public event Action OnLedgerConnected;
    public event Action OnLedgerDisconnected;

    private LedgerWallet ledgerWallet;
    private PeriodicUpdateManager periodicUpdateManager;

    private bool connected;

    public float UpdateInterval => 2f;

    [Inject]
    public void Construct(LedgerWallet ledgerWallet, PeriodicUpdateManager periodicUpdateManager)
    {
        this.ledgerWallet = ledgerWallet;
        this.periodicUpdateManager = periodicUpdateManager;
    }

    private void OnEnable()
    {
        periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    private void OnDisable()
    {
        periodicUpdateManager.RemovePeriodicUpdater(this);
    }

    public async void PeriodicUpdate()
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();
        var address = ledgerManager == null ? null : await ledgerManager.GetAddressAsync(0, 0).ConfigureAwait(false);

        if (string.IsNullOrEmpty(address))
        {
            if (connected)
                OnLedgerDisconnected?.Invoke();

            connected = false;
        }
        else
        {
            if (!connected)
                OnLedgerConnected?.Invoke();

            connected = true;
        }
    }
}