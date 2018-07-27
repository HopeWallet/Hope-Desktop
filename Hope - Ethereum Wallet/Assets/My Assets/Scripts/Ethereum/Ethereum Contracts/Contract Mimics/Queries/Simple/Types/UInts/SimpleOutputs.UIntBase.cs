using Nethereum.ABI.FunctionEncoding.Attributes;

public static partial class SimpleOutputs
{
    public abstract class UIntBase : IFunctionOutputDTO
    {
        public abstract dynamic Value { get; set; }
    }
}