using Hope.Security.ProtectedTypes.Types.Base;

namespace Hope.Security.ProtectedTypes.Types
{

    public sealed class DisposableInt : DisposableData<int>
    {
        public DisposableInt(int value) : base(value)
        {
        }
    }
}
