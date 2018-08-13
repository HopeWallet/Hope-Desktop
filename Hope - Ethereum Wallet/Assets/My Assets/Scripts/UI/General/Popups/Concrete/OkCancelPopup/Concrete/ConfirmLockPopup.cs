using System.Linq;
using TMPro;
using Zenject;

/// <summary>
/// Class used for displaying the confirmation to lock purpose.
/// </summary>
public sealed class ConfirmLockPopup : ConfirmTransactionPopupBase<ConfirmLockPopup>
{
    public TMP_Text lockPeriodText,
                    prpsAmountText,
                    dubiAmountText,
                    noteText;

    private TradableAssetManager tradableAssetManager;
    private TokenContractManager tokenContractManager;
    private TokenListManager tokenListManager;
    private DUBI dubiContract;

    /// <summary>
    /// Adds required dependencies to this class.
    /// </summary>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="tokenContractManager"> The active TokenContractManager. </param>
    /// <param name="tokenListManager"> The active TokenListManager. </param>
    /// <param name="dubiContract"> The active DUBI contract. </param>
    [Inject]
    public void Construct(
        TradableAssetManager tradableAssetManager,
        TokenContractManager tokenContractManager,
        TokenListManager tokenListManager,
        DUBI dubiContract)
    {
        this.tradableAssetManager = tradableAssetManager;
        this.tokenContractManager = tokenContractManager;
        this.tokenListManager = tokenListManager;
        this.dubiContract = dubiContract;
    }

    /// <summary>
    /// Passes the amount of purpose being locked through to display.
    /// </summary>
    /// <param name="transactionInput"> The transaction input containing the amount of purpose to lock. </param>
    protected override void InternalSetConfirmationValues(object[] transactionInput) => SetLockPrpsValues((int)transactionInput[0], (decimal)transactionInput[1]);

    /// <summary>
    /// Displays the amount of purpose and time period the purpose will be locked for.
    /// </summary>
    /// <param name="lockPeriod"> The number of months the purpose will be locked for. </param>
    /// <param name="lockAmount"> The amount of purpose to lock. </param>
    private void SetLockPrpsValues(int lockPeriod, decimal lockAmount)
    {
        lockPeriodText.text = lockPeriod + " MONTH LOCK";
        prpsAmountText.text = lockAmount.ConvertDecimalToString().LimitEnd(13, "...");
        dubiAmountText.text = (lockAmount * ((decimal)lockPeriod / 300)).ConvertDecimalToString().LimitEnd(13, "...");
        noteText.text = "You will be able to release your locked purpose after " + lockPeriod + " months have passed.";
    }

    /// <summary>
    /// Adds DUBI to the <see cref="TokenContractManager"/> if it is not already added.
    /// </summary>
    protected override void OnOkClicked()
    {
        if (!tradableAssetManager.TradableAssets.Any(pair => pair.Value.AssetAddress.EqualsIgnoreCase(dubiContract.ContractAddress)))
        {
            if (tokenListManager.ContainsToken(dubiContract.ContractAddress))
                tokenListManager.UpdateToken(dubiContract.ContractAddress, true, true);
            else
                tokenListManager.AddToken(dubiContract.ContractAddress, "Decentralized Universal Basic Income", "DUBI", 18, true, true);

            tokenContractManager.AddToken(tokenListManager.GetToken(dubiContract.ContractAddress).TokenInfo);
        }
    }
}