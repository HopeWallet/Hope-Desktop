using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class SimpleOutputs
{
    [FunctionOutput]
    public sealed class Bool : IFunctionOutputDTO
    {
        [Parameter("bool", 1)]
        public bool Value { get; set; }
    }
}