using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Zenject;

public class PRPSHodlPopup : ExitablePopupComponent<PRPSHodlPopup>, IPeriodicUpdater
{

    public Text prpsBalanceText;

    private readonly List<HodlerItem> items = new List<HodlerItem>();

    private HodlerContract hodlerContract;
    private TradableAssetManager tradableAssetManager;
    private UserWalletManager userWalletManager;
    private PeriodicUpdateManager periodicUpdateManager;
    private EthereumNetwork ethereumNetwork;

    public float UpdateInterval => 10f;

    [Inject]
    public void Construct(HodlerContract hodlerContract, 
        TradableAssetManager tradableAssetManager, 
        UserWalletManager userWalletManager, 
        PeriodicUpdateManager periodicUpdateManager,
        EthereumNetworkManager ethereumNetworkManager)
    {
        this.hodlerContract = hodlerContract;
        this.tradableAssetManager = tradableAssetManager;
        this.userWalletManager = userWalletManager;
        this.periodicUpdateManager = periodicUpdateManager;
        ethereumNetwork = ethereumNetworkManager.CurrentNetwork;
    }

    private void Awake()
    {
        StartItemSearch();
        AssignUiValues();
    }

    private void OnEnable() => periodicUpdateManager.AddPeriodicUpdater(this);

    private void OnDisable() => periodicUpdateManager.RemovePeriodicUpdater(this);

    public void PeriodicUpdate() => StartItemSearch();

    private void AssignUiValues()
    {
        prpsBalanceText.text = tradableAssetManager.ActiveTradableAsset.AssetBalance + "";
    }

    private void StartItemSearch()
    {
        WebClientUtils.GetTransactionList(ethereumNetwork.Api.GetTokenTransfersFromAndToUrl(tradableAssetManager.ActiveTradableAsset.AssetAddress,
                                                                                    userWalletManager.WalletAddress,
                                                                                    hodlerContract.ContractAddress),
                                                                                    txList => ProcessTxList(txList));
    }

    private void ProcessTxList(string txList)
    {
        var transactionsJson = JsonUtils.GetJsonData<EtherscanAPIJson<TokenTransactionJson>>(txList);
        if (transactionsJson == null)
            return;

        transactionsJson.result.ForEach(json => GetTransactionInputData(json));
    }

    private void GetTransactionInputData(TokenTransactionJson tokenTransactionJson)
    {
        TransactionUtils.CheckTransactionDetails(tokenTransactionJson.transactionHash, tx => GetItem(SolidityUtils.ExtractFunctionParameters(tx.Input)));
    }

    private void GetItem(string[] inputData)
    {
        hodlerContract.GetItem(userWalletManager.WalletAddress, inputData[1].ConvertFromHex(), item =>
        {
            if (items.Select(i => i.ReleaseTime).Count() == 0)
                items.Add(item);
        });
    }

}