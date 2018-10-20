using Ledger.Net.Connectivity;
using Nethereum.HdWallet;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class used for displaying the menu for opening the ledger wallet.
/// </summary>
public sealed class OpenLedgerWalletMenu : Menu<OpenLedgerWalletMenu>, IPeriodicUpdater
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
    /// The interval in which to recheck if the ledger is plugged in.
    /// </summary>
    public float UpdateInterval => 2f;

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
        openLedgerWalletButton.onClick.AddListener(OpenWallet);
    }

    /// <summary>
    /// Adds the periodic updater and subscribes the UserWalletManager events.
    /// </summary>
    private void OnEnable()
    {
        UserWalletManager.OnWalletLoadSuccessful += OpenMainWalletMenu;
        UserWalletManager.OnWalletLoadUnsuccessful += OnWalletLoadUnsuccessful;
        periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    /// <summary>
    /// Removes the periodic updater and unsubscribes all UserWalletManager events.
    /// </summary>
    private void OnDisable()
    {
        UserWalletManager.OnWalletLoadSuccessful -= OpenMainWalletMenu;
        UserWalletManager.OnWalletLoadUnsuccessful -= OnWalletLoadUnsuccessful;
        periodicUpdateManager.RemovePeriodicUpdater(this);
    }

    /// <summary>
    /// Opens the OpenWalletMenu if the ledger loads successful.
    /// </summary>
    private void OpenMainWalletMenu()
    {
        uiManager.OpenMenu<OpenWalletMenu>();
        uiManager.DestroyUnusedMenus();
    }

    /// <summary>
    /// Starts the load of the ledger wallet.
    /// </summary>
    private void OpenWallet()
    {
        OnHardwareWalletLoadStart?.Invoke();
        ledgerWallet.InitializeAddresses();
    }

    /// <summary>
    /// Called if the ledger is not loaded successfully.
    /// </summary>
    private void OnWalletLoadUnsuccessful()
    {
        OnHardwareWalletLoadEnd?.Invoke();
    }

    /// <summary>
    /// Checks if the ledger is connected and executes the OnLedgerConnected and OnLedgerDisconnected events accordingly.
    /// </summary>
    public async void PeriodicUpdate()
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