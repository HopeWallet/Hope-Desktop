﻿using Hope.Utils.Promises;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hope.Security.ProtectedTypes.Types.Base
{
    /// <summary>
    /// Base class to use for all protected data types.
    /// </summary>
    /// <typeparam name="TType"> The type of the original data. </typeparam>
    /// <typeparam name="TDisposable"> The <see cref="DisposableData"/> of type TType. </typeparam>
    public abstract class ProtectedType<TType, TDisposable> : SecureObject, IDisposable where TDisposable : DisposableData<TType>
    {
        private readonly MemoryEncryptor memoryEncryptor;

        private TDisposable disposableData;
        private byte[] protectedData;

        private bool disposed;

        /// <summary>
        /// The string representation of this <see cref="ProtectedType"/>.
        /// </summary>
        public string EncryptedValue => protectedData.GetBase64String();

        /// <summary>
        /// Initializes the <see cref="ProtectedType"/> with type TType.
        /// </summary>
        /// <param name="value"> The value to protect. </param>
        protected ProtectedType(TType value) : this(value, null)
        {
        }

        /// <summary>
        /// Initializes the <see cref="ProtectedType"/> with the <see cref="byte"/>[] value.
        /// </summary>
        /// <param name="value"> The <see cref="byte"/>[] value to protect. </param>
        protected ProtectedType(byte[] value) : this(value, null)
        {
        }

        /// <summary>
        /// Initializes the <see cref="ProtectedType"/> with the TType and additional <see cref="SecureObject"/> instances to use to encrypt the data.
        /// </summary>
        /// <param name="value"> The value to protect. </param>
        /// <param name="encryptionObjects"> The additional <see cref="SecureObject"/> instances to apply to the encryption. </param>
        protected ProtectedType(TType value, params SecureObject[] encryptionObjects)
        {
            SecureObject[] currentEncryptionObj = new SecureObject[] { this };

            memoryEncryptor = new MemoryEncryptor(encryptionObjects == null ? currentEncryptionObj : encryptionObjects.Concat(currentEncryptionObj).ToArray());
            SetValue(value);
        }

        /// <summary>
        /// Initializes the <see cref="ProtectedType"/> with the <see cref="byte"/>[] and additional <see cref="SecureObject"/> instances to use to encrypt the data.
        /// </summary>
        /// <param name="value"> The <see cref="byte"/>[] value to protect. </param>
        /// <param name="encryptionObjects"> The additional <see cref="SecureObject"/> instances to apply to the encryption. </param>
        protected ProtectedType(byte[] value, params SecureObject[] encryptionObjects)
        {
            SecureObject[] currentEncryptionObj = new SecureObject[] { this };

            memoryEncryptor = new MemoryEncryptor(encryptionObjects == null ? currentEncryptionObj : encryptionObjects.Concat(currentEncryptionObj).ToArray());
            SetValue(value);
        }

        /// <summary>
        /// Disposes of the ProtectedType.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                disposableData?.Dispose();
                memoryEncryptor.Dispose();
                protectedData.ClearBytes();

                disposed = true;
            }
        }

        /// <summary>
        /// Creates the <see cref="DisposableData"/> version of this <see cref="ProtectedType"/>.
        /// <para> Recommended use is within a <see langword="using"/> statement. </para>
        /// <para> Example: <see langword="using"/> (var val = <see cref="CreateDisposableData"/>) { } </para>
        /// <para> MUST BE CALLED BY A METHOD WITH [SecureCaller] OR [SecureCallEnd]. </para>
        /// </summary>
        /// <returns> Returns the DisposableData of this <see cref="ProtectedType"/>. </returns>
        [SecureCaller]
        //[ReflectionProtect(typeof(DisposableData<string>))]
        public DisposableDataPromise<TType> CreateDisposableData()
        {
            DisposableDataPromise<TType> promise = new DisposableDataPromise<TType>();
            byte[] data = memoryEncryptor.Decrypt(protectedData);

            AsyncTaskScheduler.Schedule(() => Task.Run(() =>
            {
                SetValue((byte[])data.Clone());

                GC.Collect();

                disposableData = disposableData?.Disposed != false ? (TDisposable)Activator.CreateInstance(typeof(TDisposable), data) : disposableData;
                MainThreadExecutor.QueueAction(() => promise.Build(() => disposableData));
            }));

            return promise;
        }

        /// <summary>
        /// Sets and updates the value of this <see cref="ProtectedType"/>.
        /// Value cannot be set or updated while the <see cref="DisposableData"/> is still not disposed of.
        /// This is why it is recommended to use <see cref="CreateDisposableData"/>() within a <see langword="using"/> statement since it is disposed of automatically this way.
        /// </summary>
        /// <param name="value"> The new value to set the <see cref="ProtectedType"/> to. </param>
        [SecureCallEnd]
        //[ReflectionProtect]
        public void SetValue(TType value)
        {
            SetValue(GetBytes(value));
        }

        /// <summary>
        /// Sets and updates the byte value of this <see cref="ProtectedType"/>.
        /// </summary>
        /// <param name="byteValue"> The byte value to set this <see cref="ProtectedType"/>. </param>
        [SecureCallEnd]
        //[ReflectionProtect]
        public void SetValue(byte[] byteValue)
        {
            if (disposableData?.Disposed == false)
                throw new Exception("Data can not be set while there is already a DisposableData instance active. Dispose of the data before setting new data!");
            if (byteValue == null || byteValue.Length == 0)
                throw new ArgumentNullException("Invalid value to protect!");

            protectedData?.ClearBytes();
            protectedData = memoryEncryptor.Encrypt(byteValue);

            byteValue?.ClearBytes();
        }

        /// <summary>
        /// Abstract method for retrieving the <see langword="byte"/>[] data representation of TType.
        /// </summary>
        /// <param name="value"> The value to convert to a <see langword="byte"/>[] array. </param>
        /// <returns> The converted value as a <see langword="byte"/>[] array. </returns>
        protected abstract byte[] GetBytes(TType value);
    }
}