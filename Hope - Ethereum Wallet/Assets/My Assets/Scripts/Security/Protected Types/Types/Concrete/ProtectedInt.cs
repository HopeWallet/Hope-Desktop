using Hope.Security.ProtectedTypes.Types.Base;

namespace Hope.Security.ProtectedTypes.Types
{

    public class ProtectedInt : ProtectedTypeBase<int>
    {

        public ProtectedInt(int value) : base(value)
        {
        }

        protected override int ConvertToType(string strValue) => int.Parse(strValue);

    }

}