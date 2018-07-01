using Hope.Security.Encryption.DPAPI;
using System;

namespace Hope.Security.ProtectedTypes.Types.Base
{

    /// <summary>
    /// Base class to use for all protected data types.
    /// </summary>
    /// <typeparam name="TType"> The type of the original data. </typeparam>
    /// <typeparam name="TDisposable"> The DisposableData of type TType. </typeparam>
    public abstract class ProtectedType<TType, TDisposable> where TDisposable : DisposableData<TType>
    {

        private TDisposable disposableData;
        private byte[] protectedData;

        /// <summary>
        /// The string representation of this ProtectedType.
        /// </summary>
        public string EncryptedValue => protectedData.GetBase64String();

        /// <summary>
        /// Initializes the ProtectedType with type TType.
        /// </summary>
        /// <param name="value"> The value to protect. </param>
        public ProtectedType(TType value)
        {
            SetValue(value);
        }

        /// <summary>
        /// Creates the DisposableData version of this ProtectedType.
        /// Recommended use is within a using statement. 
        /// Example: using (var val = protectedData.CreateDisposableData()) { }
        /// </summary>
        /// <returns> Returns the DisposableData of this ProtectedType. </returns>
        public TDisposable CreateDisposableData()
        {
            if (disposableData == null)
                disposableData = (TDisposable)Activator.CreateInstance(typeof(TDisposable), MemoryProtect.Unprotect(protectedData).GetUTF8String().ConvertTo<TType>());

            return disposableData;
        }

        /// <summary>
        /// Sets and updates the value of this ProtectedType.
        /// Value cannot be set or updated while the DisposableData is still not disposed of.
        /// This is why it is recommended to use CreateDisposableData() within a using statement since it is disposed of automatically this way.
        /// </summary>
        /// <param name="value"> The new value to set the ProtectedType to. </param>
        public void SetValue(TType value)
        {
            if (disposableData != null)
                throw new Exception("Data can not be set while there is already a DisposableData instance active. Dispose of the data before settings new data!");

            protectedData = MemoryProtect.Protect(value.ToString().GetUTF8Bytes());
        }
    }

}