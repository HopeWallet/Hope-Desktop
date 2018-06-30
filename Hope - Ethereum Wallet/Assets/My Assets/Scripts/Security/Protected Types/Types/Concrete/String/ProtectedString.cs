using Hope.Security.ProtectedTypes.Types.Base;

namespace Hope.Security.ProtectedTypes.Types
{

    public sealed class ProtectedString : ProtectedType<string, DisposableString>
    {
        public ProtectedString(string value) : base(value)
        {
        }
    }

}