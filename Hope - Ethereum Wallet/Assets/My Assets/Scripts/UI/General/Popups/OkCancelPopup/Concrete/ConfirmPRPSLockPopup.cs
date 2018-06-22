using Hope.Utils.EthereumUtils;
using System.Numerics;
using UnityEngine.UI;

public class ConfirmPRPSLockPopup : ConfirmTransactionRequestPopup<ConfirmPRPSLockPopup>
{

    public Text lockPeriodText,
                prpsAmountText,
                dubiAmountText;

    protected override void InternalSetConfirmationValues(object[] transactionInput) 
        => SetLockPrpsValues((int)transactionInput[0], SolidityUtils.ConvertFromUInt(transactionInput[1], 18));

    private void SetLockPrpsValues(int monthLock, decimal lockAmount)
    {
        lockPeriodText.text = monthLock + " Month Lock";
        prpsAmountText.text = lockAmount.ToString().LimitEnd(20, "...");
        dubiAmountText.text = (lockAmount * ((decimal)monthLock / 300)).ToString().LimitEnd(20, "...");
    }

}