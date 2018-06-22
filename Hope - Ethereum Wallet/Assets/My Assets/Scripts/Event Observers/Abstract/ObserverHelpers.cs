/// <summary>
/// Class that contains helper methods to subscribe/unsubscribe an observable to multipler observers at once.
/// </summary>
public static class ObserverHelpers
{
    /// <summary>
    /// Subscribes an observable to two observers.
    /// </summary>
    /// <typeparam name="T1"> The type of the first EventObserver. </typeparam>
    /// <typeparam name="T2"> The type of the second EventObserver. </typeparam>
    /// <param name="observable"> The observable to add. Must be of the type required by the EventObserver. </param>
    /// <param name="obs1"> The first EventObserver to add the observable to. </param>
    /// <param name="obs2"> The second EventObserver to add the observable to. </param>
    public static void SubscribeObservables<T1, T2>(object observable, EventObserver<T1> obs1, EventObserver<T2> obs2)
    {
        obs1.SubscribeObservable((T1)observable);
        obs2.SubscribeObservable((T2)observable);
    }

    /// <summary>
    /// Subscribes an observable to three observers.
    /// </summary>
    /// <typeparam name="T1"> The type of the first EventObserver. </typeparam>
    /// <typeparam name="T2"> The type of the second EventObserver. </typeparam>
    /// <typeparam name="T3"> The type of the third EventObserver. </typeparam>
    /// <param name="observable"> The observable to add. Must be of the type required by the EventObserver. </param>
    /// <param name="obs1"> The first EventObserver to add the observable to. </param>
    /// <param name="obs2"> The second EventObserver to add the observable to. </param>
    /// <param name="obs3"> The third EventObserver to add the observable to. </param>
    public static void SubscribeObservables<T1, T2, T3>(object observable, EventObserver<T1> obs1, EventObserver<T2> obs2, EventObserver<T3> obs3)
    {
        obs1.SubscribeObservable((T1)observable);
        obs2.SubscribeObservable((T2)observable);
        obs3.SubscribeObservable((T3)observable);
    }
    /// <summary>
    /// Unsubscribes an observable from two observers.
    /// </summary>
    /// <typeparam name="T1"> The type of the first EventObserver. </typeparam>
    /// <typeparam name="T2"> The type of the second EventObserver. </typeparam>
    /// <param name="observable"> The observable to remove. Must be of the type required by the EventObserver. </param>
    /// <param name="obs1"> The first EventObserver to unsubscribe the observable from. </param>
    /// <param name="obs2"> The second EventObserver to unsubscribe the observable from. </param>
    public static void UnsubscribeObservables<T1, T2>(object observable, EventObserver<T1> obs1, EventObserver<T2> obs2)
    {
        obs1.UnsubscribeObservable((T1)observable);
        obs2.UnsubscribeObservable((T2)observable);
    }

    /// <summary>
    /// Unsubscribes an observable from three observers.
    /// </summary>
    /// <typeparam name="T1"> The type of the first EventObserver. </typeparam>
    /// <typeparam name="T2"> The type of the second EventObserver. </typeparam>
    /// <typeparam name="T3"> The type of the third EventObserver. </typeparam>
    /// <param name="observable"> The observable to remove. Must be of the type required by the EventObserver. </param>
    /// <param name="obs1"> The first EventObserver to unsubscribe the observable from. </param>
    /// <param name="obs2"> The second EventObserver to unsubscribe the observable from. </param>
    /// <param name="obs3"> The third EventObserver to unsubscribe the observable from. </param>
    public static void UnsubscribeObservables<T1, T2, T3>(object observable, EventObserver<T1> obs1, EventObserver<T2> obs2, EventObserver<T3> obs3)
    {
        obs1.UnsubscribeObservable((T1)observable);
        obs2.UnsubscribeObservable((T2)observable);
        obs3.UnsubscribeObservable((T3)observable);
    }

}