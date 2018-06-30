using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringProtected : ProtectedTypeBase<string>
{

    public StringProtected(string value) : base(value)
    {
    }

    protected override string ConvertToType(string strValue) => strValue;

}
