using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class SimpleOutputs
{
    [FunctionOutput]
    public sealed class UInt256 : UIntBase
    {
        [Parameter("uint256", 1)]
        public override dynamic Value { get; set; }
    }
}