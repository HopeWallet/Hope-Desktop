using Nethereum.JsonRpc.UnityClient;
using Org.BouncyCastle.Crypto.Digests;
using RandomNET.Secure;
using System;
using System.Collections.Generic;

namespace Hope.Utils.Ethereum
{
    public abstract class Promise<TPromise, TReturn> where TPromise : Promise<TPromise, TReturn>, new()
    {
        private static readonly Dictionary<int, TPromise> promises = new Dictionary<int, TPromise>();
        private static readonly AdvancedSecureRandom secureRandom = new AdvancedSecureRandom(new MD2Digest());

        private TReturn successVal;
        private string errorVal;

        private bool finished;

        protected event Action<TReturn> OnPromiseSuccess;
        protected event Action<string> OnPromiseError;
        protected event Action OnPromiseSuccessOrError;

        public int Id { get; private set; }

        public static TPromise CreateNew()
        {
            return new TPromise { Id = secureRandom.Next() };
        }

        protected Promise()
        {
            OnPromiseSuccess += _ => OnPromiseSuccessOrError?.Invoke();
            OnPromiseError += _ => OnPromiseSuccessOrError?.Invoke();
            OnPromiseSuccessOrError += () => PromiseFinished();
        }

        public TPromise OnSuccess(Action<TReturn> onPromiseSuccess)
        {
            if (finished && !EqualityComparer<TReturn>.Default.Equals(successVal, default(TReturn)))
                onPromiseSuccess?.Invoke(successVal);
            else
                OnPromiseSuccess += onPromiseSuccess;

            return this as TPromise;
        }

        public TPromise OnError(Action<string> onPromiseError)
        {
            if (finished && !string.IsNullOrEmpty(errorVal))
                onPromiseError?.Invoke(errorVal);
            else
                OnPromiseError += onPromiseError;

            return this as TPromise;
        }

        public TPromise OnSuccessOrError(Action onPromiseSuccessOrError)
        {
            if (finished)
                onPromiseSuccessOrError?.Invoke();
            else
                OnPromiseSuccessOrError += onPromiseSuccessOrError;

            return this as TPromise;
        }

        public void Build<T>(UnityRequest<T> request, params Func<object>[] args)
        {
            if (request.Exception == null && !EqualityComparer<T>.Default.Equals(request.Result, default(T)))
                InternalBuild(args);
            else
                OnPromiseError?.Invoke(request.Exception.Message);
        }

        protected void InternalInvokeSuccess(TReturn returnVal)
        {
            if (finished)
                return;

            OnPromiseSuccess?.Invoke(returnVal);
            successVal = returnVal;
        }

        protected void InternalInvokeError(string errorMessage)
        {
            if (finished)
                return;

            OnPromiseError?.Invoke(errorMessage);
            errorVal = errorMessage;
        }

        private void PromiseFinished()
        {
            promises.Remove(Id);
            finished = true;
        }

        protected abstract void InternalBuild(params Func<object>[] args);
    }
}