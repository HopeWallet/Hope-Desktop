using Hope.Utils.Promises;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public abstract class ApiService
{
    private struct QueuedRequest
    {
        public string urlRequest;
        public SimplePromise<string> result;
    }

    private readonly Queue<QueuedRequest> queuedRequests = new Queue<QueuedRequest>();

    private readonly int maximumRequestsPerInterval;
    private readonly float timeInterval;

    private int sentRequestCount;

    private WaitForSeconds waitForSeconds;

    protected abstract string ApiUrl { get; }

    protected abstract int MaximumCallsPerMinute { get; }

    protected ApiService()
    {
        float callsPerSecond = MaximumCallsPerMinute / 60f;

        timeInterval = callsPerSecond > 1f ? 1f : 1f / callsPerSecond;
        maximumRequestsPerInterval = callsPerSecond < 1f ? 1 : (int)callsPerSecond;

        ApiTimerCoroutine().StartCoroutine();
    }

    protected string BuildRequest(string arguments)
    {
        return ApiUrl + arguments;
    }

    protected SimplePromise<string> SendRequest(string request)
    {
        var promise = new SimplePromise<string>();
        if (string.IsNullOrEmpty(request))
            return promise;

        InternalSendRequest(promise, request);
        InternalQueueRequest(promise, request);

        return promise;
    }

    private void InternalQueueRequest(SimplePromise<string> promise, string request)
    {
        int maximumSendableRequests = maximumRequestsPerInterval - sentRequestCount;
        if (maximumSendableRequests > 0)
            return;

        queuedRequests.Enqueue(new QueuedRequest { urlRequest = request, result = promise });
    }

    private void InternalSendRequest(SimplePromise<string> promise, string request)
    {
        int maximumSendableRequests = maximumRequestsPerInterval - sentRequestCount;
        if (maximumSendableRequests <= 0)
            return;

        sentRequestCount++;

        Observable.WhenAll(ObservableWWW.Get(request))
                  .Subscribe(results => promise.ResolveResult(results[0]), ex => promise.ResolveException(ex));
    }

    private IEnumerator ApiTimerCoroutine()
    {
        yield return waitForSeconds ?? (waitForSeconds = new WaitForSeconds(timeInterval));

        sentRequestCount = 0;

        if (queuedRequests.Count > 0)
        {
            var request = queuedRequests.Dequeue();
            InternalSendRequest(request.result, request.urlRequest);
        }

        ApiTimerCoroutine().StartCoroutine();
    }
}