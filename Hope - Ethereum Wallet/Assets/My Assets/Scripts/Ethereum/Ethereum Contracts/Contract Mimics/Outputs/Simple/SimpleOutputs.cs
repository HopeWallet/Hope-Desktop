using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SimpleOutputs
{
    public abstract class UIntBase : IFunctionOutputDTO
    {
        public abstract dynamic Value { get; set; }
    }

    [FunctionOutput]
    public sealed class UInt256 : UIntBase
    {
        [Parameter("uint256", 1)]
        public override dynamic Value { get; set; }
    }

    [FunctionOutput]
    public sealed class UInt8 : UIntBase
    {
        [Parameter("uint8", 1)]
        public override dynamic Value { get; set; }
    }

    [FunctionOutput]
    public sealed class String : IFunctionOutputDTO
    {
        [Parameter("string", 1)]
        public string Value { get; set; }
    }

    [FunctionOutput]
    public sealed class Bool : IFunctionOutputDTO
    {
        [Parameter("bool", 1)]
        public bool Value { get; set; }
    }
}