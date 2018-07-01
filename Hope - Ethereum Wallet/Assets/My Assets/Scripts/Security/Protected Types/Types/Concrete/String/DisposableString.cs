using Hope.Security.ProtectedTypes.Types.Base;

namespace Hope.Security.ProtectedTypes.Types
{

    public sealed class DisposableString : DisposableData<string>
    {

        public override string Value => unprotectedBytes.GetUTF8String();

        public DisposableString(byte[] data) : base(data)
        {
        }
    }
}
