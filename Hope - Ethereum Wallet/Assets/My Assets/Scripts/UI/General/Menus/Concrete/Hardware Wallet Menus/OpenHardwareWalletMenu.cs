using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

public abstract class OpenHardwareWalletMenu<T> : Menu<T>, IPeriodicUpdater where T : Menu<T>
{
    public event Action OnHardwareWalletLoadStart;
    public event Action OnHardwareWalletLoadEnd;

    public event Action OnHardwareWalletConnected;
    public event Action OnHardwareWalletDisconnected;

    private PeriodicUpdateManager periodicUpdateManager;

    /// <summary>
    /// The interval in which to recheck if the hardware wallet is plugged in.
    /// </summary>
    public float UpdateInterval => 2f;

    /// <summary>
    /// Adds required dependencies.
    /// </summary>
    /// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
    [Inject]
    public void Construct(PeriodicUpdateManager periodicUpdateManager)
    {
        this.periodicUpdateManager = periodicUpdateManager;
    }

    /// <summary>
    /// Adds the periodic updater and subscribes the UserWalletManager events.
    /// </summary>
    private void OnEnable()
    {
        UserWalletManager.OnWalletLoadSuccessful += OnWalletLoadSuccessful;
        UserWalletManager.OnWalletLoadUnsuccessful += OnWalletLoadUnsuccessful;
        periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    /// <summary>
    /// Removes the periodic updater and unsubscribes all UserWalletManager events.
    /// </summary>
    private void OnDisable()
    {
        UserWalletManager.OnWalletLoadSuccessful -= OnWalletLoadSuccessful;
        UserWalletManager.OnWalletLoadUnsuccessful -= OnWalletLoadUnsuccessful;
        periodicUpdateManager.RemovePeriodicUpdater(this);
    }

    /// <summary>
    /// Opens the OpenWalletMenu if the ledger loads successful.
    /// </summary>
    private void OnWalletLoadSuccessful()
    {
        uiManager.OpenMenu<OpenWalletMenu>();
        uiManager.DestroyUnusedMenus();
    }

    /// <summary>
    /// Called if the ledger is not loaded successfully.
    /// </summary>
    private void OnWalletLoadUnsuccessful()
    {
        OnHardwareWalletLoadEnd?.Invoke();
    }

    public abstract void PeriodicUpdate();
}