using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Base class used in menus which open hardware wallets.
/// </summary>
/// <typeparam name="TMenu"> The concrete hardware wallet menu class. </typeparam>
/// <typeparam name="TWallet"> The type of HardwareWallet. </typeparam>
public abstract class OpenHardwareWalletMenu<TMenu, TWallet> : Menu<TMenu>, IPeriodicUpdater

    where TMenu : Menu<TMenu>

    where TWallet : HardwareWallet
{
    public event Action OnHardwareWalletLoadStart;
    public event Action OnHardwareWalletLoadEnd;

    public event Action OnHardwareWalletConnected;
    public event Action OnHardwareWalletDisconnected;

    [SerializeField] private Button openWalletButton;

    private TWallet hardwareWallet;
    private PeriodicUpdateManager periodicUpdateManager;

    private bool wasConnected,
                 isPolling,
                 initializing;

    /// <summary>
    /// The interval in which to recheck if the hardware wallet is plugged in.
    /// </summary>
    public float UpdateInterval => 1f;

    /// <summary>
    /// Adds required dependencies.
    /// </summary>
    /// <param name="hardwareWallet"> The hardware wallet to load. </param>
    /// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
    [Inject]
    public void Construct(TWallet hardwareWallet, PeriodicUpdateManager periodicUpdateManager)
    {
        this.hardwareWallet = hardwareWallet;
        this.periodicUpdateManager = periodicUpdateManager;
    }

    /// <summary>
    /// Adds the button listener to the open ledger wallet button.
    /// </summary>
    private void Start()
    {
        openWalletButton.onClick.AddListener(StartWalletLoad);
    }

    /// <summary>
    /// Adds the periodic updater and subscribes the UserWalletManager events.
    /// </summary>
    protected virtual void OnEnable()
    {
        UserWalletManager.OnWalletLoadSuccessful += OnWalletLoadSuccessful;
        UserWalletManager.OnWalletLoadUnsuccessful += OnWalletLoadUnsuccessful;

        periodicUpdateManager.AddPeriodicUpdater(this);
    }

    /// <summary>
    /// Removes the periodic updater and unsubscribes all UserWalletManager events.
    /// </summary>
    protected virtual void OnDisable()
    {
        UserWalletManager.OnWalletLoadSuccessful -= OnWalletLoadSuccessful;
        UserWalletManager.OnWalletLoadUnsuccessful -= OnWalletLoadUnsuccessful;

        periodicUpdateManager.RemovePeriodicUpdater(this);
    }

    /// <summary>
    /// Starts the load of the hardware wallet.
    /// </summary>
    private void StartWalletLoad()
    {
        initializing = true;

        OnHardwareWalletLoadStart?.Invoke();

        CoroutineUtils.ExecuteAfterWait(1f, hardwareWallet.InitializeAddresses);
    }

    /// <summary>
    /// Opens the OpenWalletMenu if the hardware wallet loads successful.
    /// </summary>
    private void OnWalletLoadSuccessful()
    {
        uiManager.OpenMenu<OpenWalletMenu>();
        uiManager.DestroyUnusedMenus();
    }

    /// <summary>
    /// Called if the hardware wallet is not loaded successfully.
    /// </summary>
    private void OnWalletLoadUnsuccessful()
    {
        OnHardwareWalletLoadEnd?.Invoke();

        initializing = false;
        wasConnected = false;
    }

    /// <summary>
    /// Checks if the hardware wallet is connected and invokes any required events.
    /// </summary>
    public async void PeriodicUpdate()
    {
        if (isPolling || initializing)
            return;

        isPolling = true;

        bool isConnected = await IsHardwareWalletConnected().ConfigureAwait(false);

        if (!isConnected)
        {
            if (wasConnected)
                MainThreadExecutor.QueueAction(() => OnHardwareWalletDisconnected?.Invoke());
        }
        else
        {
            if (!wasConnected)
                MainThreadExecutor.QueueAction(() => OnHardwareWalletConnected?.Invoke());
        }

        wasConnected = isConnected;
        isPolling = false;
    }

    /// <summary>
    /// Abstract method used to check if the hardware wallet is connected.
    /// </summary>
    /// <returns> Task returning a bool representing the connected status. </returns>
    protected abstract Task<bool> IsHardwareWalletConnected();
}