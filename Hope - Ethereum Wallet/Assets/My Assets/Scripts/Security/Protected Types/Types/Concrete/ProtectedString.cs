using Hope.Security.ProtectedTypes.Types.Base;

namespace Hope.Security.ProtectedTypes.Types
{

    public class ProtectedString : ProtectedTypeBase<string>
    {

        public ProtectedString(string value) : base(value)
        {
        }

        protected override string ConvertToType(string strValue) => strValue;

    }

}