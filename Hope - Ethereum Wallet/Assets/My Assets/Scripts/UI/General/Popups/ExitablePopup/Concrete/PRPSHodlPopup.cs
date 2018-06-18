using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

public class PRPSHodlPopup : ExitablePopupComponent<PRPSHodlPopup>
{

    private readonly List<HodlerItem> items = new List<HodlerItem>();

    private HodlerContract hodlerContract;
    private TradableAssetManager tradableAssetManager;
    private UserWalletManager userWalletManager;
    private EthereumNetwork ethereumNetwork;

    [Inject]
    public void Construct(HodlerContract hodlerContract, 
        TradableAssetManager tradableAssetManager, 
        UserWalletManager userWalletManager, 
        EthereumNetworkManager ethereumNetworkManager)
    {
        this.hodlerContract = hodlerContract;
        this.tradableAssetManager = tradableAssetManager;
        this.userWalletManager = userWalletManager;
        ethereumNetwork = ethereumNetworkManager.CurrentNetwork;
    }

    private void OnEnable()
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
        hodlerContract.GetItem(userWalletManager.WalletAddress, inputData[1].ConvertFromHex(), item => items.Add(item));
    }

}