using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class SimpleOutputs
{
    /// <summary>
    /// Class which acts as a bytes32 return type for solidity functions.
    /// </summary>
    [FunctionOutput]
    public sealed class Bytes : IFunctionOutputDTO
    {
        /// <summary>
        /// The value of the bytes32 return type.
        /// </summary>
        [Parameter("bytes32", 1)]
        public byte[] Value { get; set; }
    }
}