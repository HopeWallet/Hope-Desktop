using System.Collections;
using System.Collections.Generic;

public class Hodler : FixedContract<HodlerContract>
{
}

public class HodlerContract : ContractBase
{

    private const string FUNC_HODL = "hodl";
    private const string FUNC_RELEASE = "release";
    private const string FUNC_GETITEM = "getItem";

    protected override string[] FunctionNames => new string[] { FUNC_HODL, FUNC_RELEASE, FUNC_GETITEM };

    public HodlerContract(string contractAddress, string abi) : base(contractAddress, abi)
    {
    }

}
