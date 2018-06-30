using Hope.Security.ProtectedTypes.Types.Base;

namespace Hope.Security.ProtectedTypes.Types
{

    public sealed class ProtectedInt : ProtectedType<int, DisposableInt>
    {
        public ProtectedInt(int value) : base(value)
        {
        }
    }

}