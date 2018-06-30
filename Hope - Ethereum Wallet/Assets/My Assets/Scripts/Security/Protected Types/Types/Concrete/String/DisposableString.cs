using Hope.Security.ProtectedTypes.Types.Base;

namespace Hope.Security.ProtectedTypes.Types
{

    public sealed class DisposableString : DisposableData<string>
    {
        public DisposableString(string value) : base(value)
        {
        }
    }
}
