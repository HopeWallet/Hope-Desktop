using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class SimpleOutputs
{
    [FunctionOutput]
    public sealed class UInt32 : UIntBase
    {
        [Parameter("uint32", 1)]
        public override dynamic Value { get; set; }
    }
}