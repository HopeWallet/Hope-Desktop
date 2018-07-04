﻿using Hope.Security.Encryption.DPAPI;
using System;

namespace Hope.Security.ProtectedTypes.Types.Base
{
    /// <summary>
    /// Base class to use for all protected data types.
    /// </summary>
    /// <typeparam name="TType"> The type of the original data. </typeparam>
    /// <typeparam name="TDisposable"> The <see cref="DisposableData"/> of type TType. </typeparam>
    public abstract class ProtectedType<TType, TDisposable> where TDisposable : DisposableData<TType>
    {
        private TDisposable disposableData;
        private byte[] protectedData;

        /// <summary>
        /// The string representation of this <see cref="ProtectedType"/>.
        /// </summary>
        public string EncryptedValue => protectedData.GetBase64String();

        /// <summary>
        /// Initializes the <see cref="ProtectedType"/> with type TType.
        /// </summary>
        /// <param name="value"> The value to protect. </param>
        protected ProtectedType(TType value)
        {
            SetValue(value);
        }

        /// <summary>
        /// Creates the <see cref="DisposableData"/> version of this <see cref="ProtectedType"/>.
        /// Recommended use is within a <see langword="using"/> statement. 
        /// Example: <see langword="using"/> (var val = <see cref="CreateDisposableData"/>) { }
        /// </summary>
        /// <returns> Returns the DisposableData of this <see cref="ProtectedType"/>. </returns>
        public TDisposable CreateDisposableData()
        {
            return disposableData ?? (disposableData = (TDisposable)Activator.CreateInstance(typeof(TDisposable), MemoryProtect.Unprotect(protectedData)));
        }

        /// <summary>
        /// Sets and updates the value of this <see cref="ProtectedType"/>.
        /// Value cannot be set or updated while the <see cref="DisposableData"/> is still not disposed of.
        /// This is why it is recommended to use <see cref="CreateDisposableData"/>() within a <see langword="using"/> statement since it is disposed of automatically this way.
        /// </summary>
        /// <param name="value"> The new value to set the <see cref="ProtectedType"/> to. </param>
        public void SetValue(TType value)
        {
            if (disposableData != null)
                throw new Exception("Data can not be set while there is already a DisposableData instance active. Dispose of the data before settings new data!");

            protectedData = MemoryProtect.Protect(GetBytes(value));
        }

        /// <summary>
        /// Abstract method for retrieving the <see langword="byte"/>[] data representation of TType.
        /// </summary>
        /// <param name="value"> The value to convert to a <see langword="byte"/>[] array. </param>
        /// <returns> The converted value as a <see langword="byte"/>[] array. </returns>
        protected abstract byte[] GetBytes(TType value);
    }
}