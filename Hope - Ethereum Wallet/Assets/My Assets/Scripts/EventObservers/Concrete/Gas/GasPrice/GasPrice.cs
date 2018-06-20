using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
using System.Numerics;

/// <summary>
/// Struct which contains the functional gas price for transactions and readable gas price for display.
/// </summary>
public struct GasPrice
{
    /// <summary>
    /// The functional gas price to use when sending transactions.
    /// </summary>
    public HexBigInteger FunctionalGasPrice { get; private set; }

    /// <summary>
    /// The readable gas price to use for display.
    /// </summary>
    public decimal ReadableGasPrice { get; private set; }

    /// <summary>
    /// Initializes this GasPrice with the functional and readable prices.
    /// </summary>
    /// <param name="functionalGasPrice"> The functional gas price to use when sending transactions. </param>
    public GasPrice(HexBigInteger functionalGasPrice)
    {
        FunctionalGasPrice = functionalGasPrice;
        ReadableGasPrice = GasUtils.GetReadableGasPrice(functionalGasPrice.Value);
    }
}