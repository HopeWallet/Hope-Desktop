using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class TokenListManager
{
    private readonly SecurePlayerPrefList<AddableTokenJson> tokenList;

    public TokenListManager()
    {
        tokenList = new SecurePlayerPrefList<AddableTokenJson>("test");
    }
}