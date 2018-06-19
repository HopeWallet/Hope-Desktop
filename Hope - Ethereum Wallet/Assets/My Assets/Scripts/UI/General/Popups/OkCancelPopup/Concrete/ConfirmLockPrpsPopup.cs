using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class ConfirmLockPrpsPopup : OkCancelPopupComponent<ConfirmLockPrpsPopup>
{

    public Text lockPeriodText,
                prpsAmountText,
                dubiAmountText,
                timerText;

    private Action confirmPressed;

    private HexBigInteger gasLimit,
                          gasPrice;

    private int monthsToLock;

    public void SetLockPrpsValues(int monthLock, decimal lockAmount, HexBigInteger limit, HexBigInteger price, Action onConfirmPressed)
    {
        lockPeriodText.text = monthLock + " Month Lock";
        prpsAmountText.text = lockAmount.ToString().LimitEnd(20, "...");
        dubiAmountText.text = (lockAmount * ((decimal)monthLock / 300)).ToString().LimitEnd(20, "...");
        monthsToLock = monthLock;
        gasLimit = limit;
        gasPrice = price;
        confirmPressed = onConfirmPressed;
    }

    /// <summary>
    /// Initializes the component by getting all required components used to display the transaction info.
    /// </summary>
    protected override void OnStart()
    {
        new CountdownTimer(time => timerText.text = time.ToString(), () => { okButton.interactable = true; timerText.text = ""; }, 5f, 1f).StartCountdown();
    }

    /// <summary>
    /// Called when the confirm button is clicked, which executes the transfer of the asset.
    /// </summary>
    protected override void OnOkClicked() => confirmPressed?.Invoke();

}