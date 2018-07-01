using Hope.Security.ProtectedTypes.Types.Base;
using System;

namespace Hope.Security.ProtectedTypes.Types
{

    public sealed class DisposableInt : DisposableData<int>
    {

        public override int Value => BitConverter.ToInt32(unprotectedBytes, 0);

        public DisposableInt(byte[] data) : base(data)
        {
        }
    }
}
