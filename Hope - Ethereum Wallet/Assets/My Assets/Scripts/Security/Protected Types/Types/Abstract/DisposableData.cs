using System;

namespace Hope.Security.ProtectedTypes.Types.Base
{

    /// <summary>
    /// Class which contains data which can be disposed of.
    /// </summary>
    /// <typeparam name="T"> The type of the data. </typeparam>
    public abstract class DisposableData<T> : IDisposable
    {

        protected readonly byte[] unprotectedBytes;

        private bool disposedValue = false;

        /// <summary>
        /// The value belonging to this DisposableData.
        /// </summary>
        public abstract T Value { get; }

        /// <summary>
        /// Initializes this DisposableData with the value.
        /// </summary>
        /// <param name="data"> The byte array of this DisposableData type. </param>
        public DisposableData(byte[] data)
        {
            unprotectedBytes = data;
        }

        /// <summary>
        /// Disposes of the value this DisposableData class holds and cleans all garbage.
        /// </summary>
        public void Dispose()
        {
            if (!disposedValue)
            {
                if (unprotectedBytes != null)
                    Array.Clear(unprotectedBytes, 0, unprotectedBytes.Length);

                disposedValue = true;
            }

            GC.SuppressFinalize(this);
            GC.Collect();
        }

    }
}