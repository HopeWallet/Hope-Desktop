using Hope.Security.Encryption.DPAPI;
using System;

namespace Hope.Security.ProtectedTypes.Types.Base
{

    public abstract class ProtectedTypeBase<T>
    {

        private DisposableData<T> disposableData;

        private byte[] protectedData;

        public string EncryptedValue => protectedData.GetBase64String();

        public ProtectedTypeBase(T value)
        {
            SetValue(value);
        }

        public DisposableData<T> GetDisposableData()
        {
            if (disposableData == null)
                disposableData = new DisposableData<T>(GetUnprotectedData());

            return disposableData;
        }

        public void SetValue(T value)
        {
            if (disposableData != null)
                throw new Exception("Data can not be set while there is already a DisposableData instance active. Dispose of the data before settings new data!");

            protectedData = GetProtectedData(value);
            disposableData = new DisposableData<T>(value);

        }

        private T GetUnprotectedData() => ConvertToType(MemoryProtect.Unprotect(protectedData).GetUTF8String());

        private byte[] GetProtectedData(T value) => MemoryProtect.Protect(value.ToString().GetUTF8Bytes());

        protected abstract T ConvertToType(string strValue);

    }

}