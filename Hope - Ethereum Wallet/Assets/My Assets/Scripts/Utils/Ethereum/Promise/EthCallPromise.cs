using System;

namespace Hope.Utils.Ethereum
{
    public sealed class EthCallPromise<T> : Promise<EthCallPromise<T>, T>
    {
        protected override void InternalBuild(params Func<object>[] args)
        {
            InvokeSuccess((T)args[0]?.Invoke());
        }
    }
}