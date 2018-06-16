using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

/// <summary>
/// Class which contains the data for an order on DubiEx.
/// </summary>
[FunctionOutput]
public class DubiExOrder
{

	/// <summary>
	/// The id of this order.
	/// </summary>
	[Parameter("uint256", "id", 1)]
	public BigInteger Id { get; set; }

	/// <summary>
	/// The address of whoever made this order.
	/// </summary>
	[Parameter("address", "maker", 2)]
	public string Maker { get; set; }

	/// <summary>
	/// The amount.
	/// </summary>
	[Parameter("uint256", "amount", 3)]
	public BigInteger Amount { get; set; }

	/// <summary>
	/// The first token pair for the order.
	/// </summary>
	[Parameter("address", "pairA", 4)]
	public string PairA { get; set; }

	/// <summary>
	/// The second token pair for the order.
	/// </summary>
	[Parameter("address", "pairB", 5)]
	public string PairB { get; set; }

	/// <summary>
	/// The price of the token of the first pair.
	/// </summary>
	[Parameter("uint256", "priceA", 6)]
	public BigInteger PriceA { get; set; }

	/// <summary>
	/// The price of the token of the second pair.
	/// </summary>
	[Parameter("uint256", "priceB", 7)]
	public BigInteger PriceB { get; set; }

}