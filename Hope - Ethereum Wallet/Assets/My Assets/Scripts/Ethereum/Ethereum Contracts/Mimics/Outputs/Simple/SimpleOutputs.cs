using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SimpleOutputs
{
    [FunctionOutput]
    public sealed class UInt256 : IFunctionOutputDTO
    {
        [Parameter("uint256", 1)]
        public dynamic Value { get; set; }
    }

    [FunctionOutput]
    public sealed class UInt8 : IFunctionOutputDTO
    {
        [Parameter("uint8", 1)]
        public dynamic Value { get; set; }
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