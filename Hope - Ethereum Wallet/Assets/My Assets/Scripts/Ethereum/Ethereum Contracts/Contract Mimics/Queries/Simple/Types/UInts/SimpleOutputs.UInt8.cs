using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class SimpleOutputs
{
    [FunctionOutput]
    public sealed class UInt8 : UIntBase
    {
        [Parameter("uint8", 1)]
        public override dynamic Value { get; set; }
    }
}