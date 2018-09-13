using Hope.Utils.Promises;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// Base class to use to send calls to api urls while staying within the allowed call amount per second.
/// </summary>
public abstract class ApiService
{
    /// <summary>
    /// Struct which represents a query request to be sent.
    /// </summary>
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

    /// <summary>
    /// The base url of the api.
    /// </summary>
    protected abstract string ApiUrl { get; }

    /// <summary>
    /// The maximum number of allowed calls per minute.
    /// </summary>
    protected abstract int MaximumCallsPerMinute { get; }

    /// <summary>
    /// Initializes the ApiService by assigning all timing variables and starting the api timer.
    /// </summary>
    protected ApiService()
    {
        float callsPerSecond = MaximumCallsPerMinute / 60f;

        timeInterval = callsPerSecond > 1f ? 1f : 1f / callsPerSecond;
        maximumRequestsPerInterval = callsPerSecond < 1f ? 1 : (int)callsPerSecond;

        ApiTimerCoroutine().StartCoroutine();
    }

    /// <summary>
    /// Builds the api url with the arguments.
    /// </summary>
    /// <param name="arguments"> The api arguments to add to the request. </param>
    /// <returns> The api request. </returns>
    protected string BuildRequest(string arguments)
    {
        return ApiUrl + arguments;
    }

    /// <summary>
    /// Sends a request if this ApiService is under the limit. If the limit has been exceeded, the request will be added to queue.
    /// </summary>
    /// <param name="request"> The api request to send. </param>
    /// <returns> The promise of the result from the api. </returns>
    protected SimplePromise<string> SendRequest(string request)
    {
        var promise = new SimplePromise<string>();
        if (string.IsNullOrEmpty(request))
            return promise;

        InternalQueueRequest(promise, request);
        InternalSendRequest(promise, request);

        return promise;
    }

    /// <summary>
    /// Queues a request if there are no sendable requests available.
    /// </summary>
    /// <param name="promise"> The promise returning the result of this request. </param>
    /// <param name="request"> The api request to send. </param>
    private void InternalQueueRequest(SimplePromise<string> promise, string request)
    {
        int maximumSendableRequests = maximumRequestsPerInterval - sentRequestCount;
        if (maximumSendableRequests > 0)
            return;

        queuedRequests.Enqueue(new QueuedRequest { urlRequest = request, result = promise });
    }

    /// <summary>
    /// Sends the api request if we are under the maximum sendable requests in this time interval.
    /// </summary>
    /// <param name="promise"> The promise returning the result of this request. </param>
    /// <param name="request"> The api request to send. </param>
    private void InternalSendRequest(SimplePromise<string> promise, string request)
    {
        int maximumSendableRequests = maximumRequestsPerInterval - sentRequestCount;
        if (maximumSendableRequests <= 0)
            return;

        sentRequestCount++;

        Observable.WhenAll(ObservableWWW.Get(request))
                  .Subscribe(results => promise.ResolveResult(results[0]), ex => promise.ResolveException(ex));
    }

    /// <summary>
    /// Coroutine which waits the time interval and then executes the queued requests.
    /// </summary>
    private IEnumerator ApiTimerCoroutine()
    {
        yield return waitForSeconds ?? (waitForSeconds = new WaitForSeconds(timeInterval));

        sentRequestCount = 0;

        for (int i = 0; i < (queuedRequests.Count < maximumRequestsPerInterval ? queuedRequests.Count : maximumRequestsPerInterval); i++)
        {
            var request = queuedRequests.Dequeue();
            InternalSendRequest(request.result, request.urlRequest);
        }

        ApiTimerCoroutine().StartCoroutine();
    }
}