using Hope.Security.ProtectedTypes.Types.Base;
using System;

namespace Hope.Utils.Promises
{
    /// <summary>
    /// Promise of the DisposableData instance created from the ProtectedType.
    /// </summary>
    /// <typeparam name="T"> The type of the DisposableData. </typeparam>
    public sealed class DisposableDataPromise<T> : Promise<DisposableDataPromise<T>, DisposableData<T>>
    {
        /// <summary>
        /// Builds the DisposableData instance.
        /// </summary>
        /// <param name="args"> The arguments containing the DisposableData instance. </param>
        protected override void InternalBuild(params Func<object>[] args)
        {
            DisposableData<T> data = args[0]?.Invoke() as DisposableData<T>;

            if (data == null)
            {
                InternalInvokeError("DisposableData is null");
            }
            else
            {
                InternalInvokeSuccess(data);
                data.Dispose();
            }
        }
    }
}
