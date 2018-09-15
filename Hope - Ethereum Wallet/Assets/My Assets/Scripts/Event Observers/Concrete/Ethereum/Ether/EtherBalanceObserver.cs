/// <summary>
/// Class used for observing and notifying IEtherBalanceObservables what the latest ether balance is in the wallet.
/// </summary>
public class EtherBalanceObserver : EventObserver<IEtherBalanceObservable>, IPeriodicUpdater
{
    private EtherAsset etherAsset;

    private dynamic etherBalance;

    /// <summary>
    /// How often the ether balance should be updated.
    /// </summary>
    public float UpdateInterval => 10f;

    /// <summary>
    /// Initializes the EtherBalanceObserver once all tokens have been loaded.
    /// </summary>
    /// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    public EtherBalanceObserver(PeriodicUpdateManager periodicUpdateManager, TradableAssetManager tradableAssetManager)
    {
        TokenContractManager.OnTokensLoaded += () => StartObserver(periodicUpdateManager, tradableAssetManager);
    }

    /// <summary>
    /// Updates the observable's ether balance once it is added.
    /// </summary>
    /// <param name="observable"> The recently added observable. </param>
    protected override void OnObservableAdded(IEtherBalanceObservable observable) => observable.EtherBalance = etherBalance;

    /// <summary>
    /// Starts the EtherBalanceObserver.
    /// </summary>
    /// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    private void StartObserver(PeriodicUpdateManager periodicUpdateManager, TradableAssetManager tradableAssetManager)
    {
        etherAsset = tradableAssetManager.EtherAsset;
        etherAsset.OnAssetBalanceChanged += OnEtherBalanceChanged;

        periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    /// <summary>
    /// Updates the ether balance and notifies each observable if the balance changes.
    /// </summary>
    public void PeriodicUpdate() => etherAsset.UpdateBalance();

    /// <summary>
    /// Notifies observables of an ether balance change.
    /// </summary>
    /// <param name="balance"> The new ether balance. </param>
    private void OnEtherBalanceChanged(dynamic balance)
    {
        etherBalance = balance;
        observables.SafeForEach(observable => observable.EtherBalance = balance);
    }
}