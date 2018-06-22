using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EtherBalanceObserver : EventObserver<IEtherBalanceObservable>, IPeriodicUpdater
{

    private EtherAsset etherAsset;

    public float UpdateInterval => 10f;

    public EtherBalanceObserver(PeriodicUpdateManager periodicUpdateManager, TradableAssetManager tradableAssetManager)
    {
        TokenContractManager.OnTokensLoaded += () => StartObserver(periodicUpdateManager, tradableAssetManager);
    }

    private void StartObserver(PeriodicUpdateManager periodicUpdateManager, TradableAssetManager tradableAssetManager)
    {
        etherAsset = tradableAssetManager.EtherAsset;
        periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    public void PeriodicUpdate() => etherAsset.UpdateBalance(() => observables.SafeForEach(observable => observable.EtherBalance = etherAsset.AssetBalance));

}