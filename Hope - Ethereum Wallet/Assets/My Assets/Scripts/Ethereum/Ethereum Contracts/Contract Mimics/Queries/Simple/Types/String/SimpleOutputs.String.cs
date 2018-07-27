using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class SimpleOutputs
{
    [FunctionOutput]
    public sealed class String : IFunctionOutputDTO
    {
        [Parameter("string", 1)]
        public string Value { get; set; }
    }
}