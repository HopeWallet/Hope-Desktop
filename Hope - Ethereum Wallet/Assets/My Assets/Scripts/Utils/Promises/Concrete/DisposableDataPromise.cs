using Hope.Security.ProtectedTypes.Types.Base;
using System;

namespace Hope.Utils.Promises
{
    public sealed class DisposableDataPromise<T> : Promise<DisposableDataPromise<T>, DisposableData<T>>
    {
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
