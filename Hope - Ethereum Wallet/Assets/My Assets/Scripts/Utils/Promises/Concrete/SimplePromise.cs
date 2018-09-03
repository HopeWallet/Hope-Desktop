using System;

namespace Hope.Utils.Promises
{
    /// <summary>
    /// Class which acts as a simple dynamic promise which can be resolved dynamically.
    /// </summary>
    /// <typeparam name="T"> The return type of the promise. </typeparam>
    public sealed class SimplePromise<T> : Promise<SimplePromise<T>, T>
    {
        /// <summary>
        /// Resolves the promise with the given result.
        /// </summary>
        /// <param name="result"> The result to return. </param>
        /// <returns> The current SimplePromise. </returns>
        public SimplePromise<T> Resolve(T result)
        {
            InternalBuild(() => result);
            return this;
        }

        /// <summary>
        /// Builds the promise and invokes success if the first argument is not null.
        /// </summary>
        /// <param name="args"> The arguments to check. </param>
        protected override void InternalBuild(params Func<object>[] args)
        {
            var arg = args[0]?.Invoke();
            if (arg != null)
                InternalInvokeSuccess((T)arg);
            else
                InternalInvokeError("Error");
        }
    }
}