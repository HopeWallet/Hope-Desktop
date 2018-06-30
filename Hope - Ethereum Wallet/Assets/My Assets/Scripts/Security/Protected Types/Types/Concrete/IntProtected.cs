using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class IntProtected : ProtectedTypeBase<int>
{

    public IntProtected(int value) : base(value)
    {
    }

    protected override int ConvertToType(string strValue) => int.Parse(strValue);

}