using System;

/// <summary>
/// Class used to display a warning popup to the user if they currently have a transaction pending and try to send another.
/// </summary>
public sealed class TransactionWarningPopup : OkCancelPopupComponent<TransactionWarningPopup>
{
    private event Action OnOkPressed;
    private event Action OnCancelPressed;

    /// <summary>
    /// Adds an action to call if the user proceeds with the transaction signing.
    /// </summary>
    /// <param name="onProceed"> Called if the user proceeds. </param>
    /// <returns> The current TransactionWarningPopup instance. </returns>
    public TransactionWarningPopup OnProceed(Action onProceed)
    {
        OnOkPressed += onProceed;
        return this;
    }

    /// <summary>
    /// Adds an action to call if the user clicks cancel and doesn't go through with the transaction signing.
    /// </summary>
    /// <param name="onCancel"> Called if the user clicks cancel. </param>
    /// <returns> The current TransactionWarningPopup instance. </returns>
    public TransactionWarningPopup OnCancel(Action onCancel)
    {
        OnCancelPressed += onCancel;
        return this;
    }

    /// <summary>
    /// Invokes the OnOkPressed event.
    /// </summary>
    protected override void OnOkClicked() => OnOkPressed?.Invoke();

    /// <summary>
    /// Invokes the OnCancelPressed event.
    /// </summary>
    protected override void OnCancelClicked() => OnCancelPressed?.Invoke();
}