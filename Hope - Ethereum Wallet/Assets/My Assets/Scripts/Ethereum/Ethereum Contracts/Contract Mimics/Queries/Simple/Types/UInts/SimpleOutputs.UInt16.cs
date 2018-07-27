using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class SimpleOutputs
{
    [FunctionOutput]
    public sealed class UInt16 : UIntBase
    {
        [Parameter("uint16", 1)]
        public override dynamic Value { get; set; }
    }
}