using Hope.Utils.Ethereum;
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
    public HexBigInteger FunctionalGasPrice { get; }

    /// <summary>
    /// The readable gas price to use for display.
    /// </summary>
    public decimal ReadableGasPrice { get; }

    /// <summary>
    /// Initializes this GasPrice with the functional and readable prices using a <see cref="HexBigInteger"/>.
    /// </summary>
    /// <param name="functionalGasPrice"> The <see cref="HexBigInteger"/> functional gas price to use when sending transactions. </param>
    public GasPrice(HexBigInteger functionalGasPrice)
    {
        FunctionalGasPrice = functionalGasPrice;
        ReadableGasPrice = GasUtils.GetReadableGasPrice(functionalGasPrice.Value);
    }

    /// <summary>
    /// Initializes this GasPrice with the functional and readable gas prices using a <see cref="BigInteger"/>.
    /// </summary>
    /// <param name="functionalGasPrice"> The <see cref="BigInteger"/> functional gas price to use when sending transactions. </param>
    public GasPrice(BigInteger functionalGasPrice)
    {
        FunctionalGasPrice = new HexBigInteger(functionalGasPrice);
        ReadableGasPrice = GasUtils.GetReadableGasPrice(functionalGasPrice);
    }
}