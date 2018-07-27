using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SimpleOutputs
{
    [FunctionOutput]
    public sealed class UInt256OutputType : IFunctionOutputDTO
    {
        [Parameter("uint256", 1)]
        public dynamic Value { get; set; }
    }
}