using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class SimpleOutputs
{
    [FunctionOutput]
    public sealed class UInt64 : UIntBase
    {
        [Parameter("uint64", 1)]
        public override dynamic Value { get; set; }
    }
}