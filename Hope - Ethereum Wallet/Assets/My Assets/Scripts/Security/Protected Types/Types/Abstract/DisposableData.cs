using System;

namespace Hope.Security.ProtectedTypes.Types.Base
{

    public abstract class DisposableData<T> : IDisposable
    {

        private bool disposedValue = false;

        public T Value { get; private set; }

        public DisposableData(T value)
        {
            Value = value;
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                Value = default(T);
                disposedValue = true;
            }

            GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}