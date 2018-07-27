using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class SimpleOutputs
{
    [FunctionOutput]
    public sealed class UInt128 : UIntBase
    {
        [Parameter("uint128", 1)]
        public override dynamic Value { get; set; }
    }
}