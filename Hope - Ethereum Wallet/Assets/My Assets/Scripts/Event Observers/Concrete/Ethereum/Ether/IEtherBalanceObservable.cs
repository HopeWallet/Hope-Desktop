/// <summary>
/// Interface for getting updates on the current ethereum balance.
/// </summary>
public interface IEtherBalanceObservable
{

    /// <summary>
    /// The current ether balance of this wallet.
    /// </summary>
    dynamic EtherBalance { get; set; }
}