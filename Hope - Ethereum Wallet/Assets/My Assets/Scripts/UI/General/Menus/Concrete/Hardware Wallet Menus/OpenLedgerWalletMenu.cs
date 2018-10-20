using Ledger.Net.Connectivity;
using Nethereum.HdWallet;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class used for displaying the menu for opening the ledger wallet.
/// </summary>
public sealed class OpenLedgerWalletMenu : OpenHardwareWalletMenu<OpenLedgerWalletMenu>
{
    public event Action OnHardwareWalletLoadStart;
    public event Action OnHardwareWalletLoadEnd;

    public event Action OnHardwareWalletConnected;
    public event Action OnHardwareWalletDisconnected;

    [SerializeField] private Button openLedgerWalletButton;

    private LedgerWallet ledgerWallet;
    private PeriodicUpdateManager periodicUpdateManager;

    private bool connected;

    /// <summary>
    /// Adds required dependencies.
    /// </summary>
    /// <param name="ledgerWallet"> The active LedgerWallet instance. </param>
    /// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
    [Inject]
    public void Construct(LedgerWallet ledgerWallet, PeriodicUpdateManager periodicUpdateManager)
    {
        this.ledgerWallet = ledgerWallet;
        this.periodicUpdateManager = periodicUpdateManager;
    }

    /// <summary>
    /// Adds the button listener to the open ledger wallet button.
    /// </summary>
    private void Start()
    {
        openLedgerWalletButton.onClick.AddListener(StartWalletLoad);
    }

    /// <summary>
    /// Starts the load of the ledger wallet.
    /// </summary>
    private void StartWalletLoad()
    {
        OnHardwareWalletLoadStart?.Invoke();
        ledgerWallet.InitializeAddresses();
    }

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