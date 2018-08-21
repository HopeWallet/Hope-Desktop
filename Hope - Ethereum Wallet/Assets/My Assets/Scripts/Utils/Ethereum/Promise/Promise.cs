using Nethereum.JsonRpc.UnityClient;
using System;
using System.Collections.Generic;

namespace Hope.Utils.Ethereum
{
    public abstract class Promise<TPromise, TReturn> where TPromise : Promise<TPromise, TReturn>, new()
    {
        protected event Action<TReturn> OnPromiseSuccess;
        protected event Action<string> OnPromiseFail;
        protected event Action OnPromiseSuccessOrFail;

        private int id;

        private static readonly Dictionary<int, TPromise> promises = new Dictionary<int, TPromise>();

        public static TPromise GetPromise(int id)
        {
            if (!promises.ContainsKey(id))
                promises.Add(id, new TPromise { id = id });

            return promises[id];
        }

        protected Promise()
        {
            OnPromiseSuccess += _ => OnPromiseSuccessOrFail?.Invoke();
            OnPromiseFail += _ => OnPromiseSuccessOrFail?.Invoke();
            OnPromiseSuccessOrFail += () => promises.Remove(id);
        }

        public TPromise OnSuccess(Action<TReturn> onPromiseSuccess)
        {
            OnPromiseSuccess += onPromiseSuccess;
            return this as TPromise;
        }

        public TPromise OnFail(Action<string> onPromiseFail)
        {
            OnPromiseFail += onPromiseFail;
            return this as TPromise;
        }

        public TPromise OnSuccessOrFail(Action onPromiseSuccessOrFail)
        {
            OnPromiseSuccessOrFail += onPromiseSuccessOrFail;
            return this as TPromise;
        }

        public void Build<T>(UnityRequest<T> request, params Func<object>[] args)
        {
            if (request.Exception == null && !EqualityComparer<T>.Default.Equals(request.Result, default(T)))
                InternalBuild(args);
            else
                OnPromiseFail?.Invoke(request.Exception.Message);
        }

        protected void InvokeSuccess(TReturn returnVal)
        {
            OnPromiseSuccess?.Invoke(returnVal);
        }

        protected void InvokeFail(string errorMessage)
        {
            OnPromiseFail?.Invoke(errorMessage);
        }

        protected abstract void InternalBuild(params Func<object>[] args);
    }
}