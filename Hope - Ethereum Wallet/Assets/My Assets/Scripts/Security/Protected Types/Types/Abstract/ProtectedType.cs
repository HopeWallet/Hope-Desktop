using Hope.Security.Encryption.DPAPI;
using System;

namespace Hope.Security.ProtectedTypes.Types.Base
{

    /// <summary>
    /// Base class to use for all protected data types.
    /// </summary>
    /// <typeparam name="TType"> The type which  </typeparam>
    /// <typeparam name="TDisposable"></typeparam>
    public abstract class ProtectedType<TType, TDisposable> where TDisposable : DisposableData<TType>
    {

        private TDisposable disposableData;

        private byte[] protectedData;

        public string EncryptedValue => protectedData.GetBase64String();

        public ProtectedType(TType value)
        {
            SetValue(value);
        }

        public TDisposable CreateDisposableData()
        {
            if (disposableData == null)
                disposableData = (TDisposable)Activator.CreateInstance(typeof(TDisposable), GetUnprotectedData());

            return disposableData;
        }

        public void SetValue(TType value)
        {
            if (disposableData != null)
                throw new Exception("Data can not be set while there is already a DisposableData instance active. Dispose of the data before settings new data!");

            protectedData = GetProtectedData(value);
            disposableData = (TDisposable)Activator.CreateInstance(typeof(TDisposable), value);
        }

        private byte[] GetProtectedData(TType value) => MemoryProtect.Protect(value.ToString().GetUTF8Bytes());

        private TType GetUnprotectedData() => MemoryProtect.Unprotect(protectedData).GetUTF8String().ConvertTo<TType>();

    }

}