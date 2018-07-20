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

        /// <summary>
        /// The value belonging to this <see cref="DisposableData"/>.
        /// </summary>
        public abstract T Value { get; }

        /// <summary>
        /// Whether this <see cref="DisposableData"/> has been disposed of yet.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Initializes this <see cref="DisposableData"/> with the value.
        /// </summary>
        /// <param name="data"> The <see langword="byte"/>[] array of this <see cref="DisposableData"/> type. </param>
        protected DisposableData(byte[] data)
        {
            unprotectedBytes = data;
        }

        /// <summary>
        /// Disposes of the value this <see cref="DisposableData"/> class holds and cleans all garbage.
        /// </summary>
        public void Dispose()
        {
            if (!Disposed)
            {
                unprotectedBytes?.ClearBytes();
                Disposed = true;
            }

            GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}