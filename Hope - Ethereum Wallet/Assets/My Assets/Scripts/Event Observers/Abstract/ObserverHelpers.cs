using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ObserverHelpers
{

    public static void SubscribeObservable(object observable, params EventObserver<object>[] eventObservers)
    {
        foreach(EventObserver<object> observer in eventObservers)
        {
            if (observable.GetType() == observer.ObservableType)
                observer.SubscribeObservable(observable);
        }
    }

    public static void UnsubscribeObservable(object observable, params EventObserver<object>[] eventObservers)
    {
        foreach (EventObserver<object> observer in eventObservers)
        {
            if (observable.GetType() == observer.ObservableType)
                observer.UnsubscribeObservable(observable);
        }
    }

    public static void SubscribeObservables<T1, T2, T3>(object observable, EventObserver<T1> obs1, EventObserver<T2> obs2, EventObserver<T3> obs3)
    {
        obs1.SubscribeObservable((T1)observable);
        obs2.SubscribeObservable((T2)observable);
        obs3.SubscribeObservable((T3)observable);
    }

    public static void UnsubscribeObservables<T1, T2, T3>(object observable, EventObserver<T1> obs1, EventObserver<T2> obs2, EventObserver<T3> obs3)
    {
        obs1.UnsubscribeObservable((T1)observable);
        obs2.UnsubscribeObservable((T2)observable);
        obs3.UnsubscribeObservable((T3)observable);
    }

}