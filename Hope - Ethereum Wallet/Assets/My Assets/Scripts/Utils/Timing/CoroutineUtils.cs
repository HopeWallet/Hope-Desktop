using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Class used for starting coroutines from non-monobehaviour classes.
/// </summary>
public static class CoroutineUtils
{

	private static readonly AppInstaller CoroutineServiceComponent = UnityEngine.Object.FindObjectOfType<AppInstaller>();

    /// <summary>
    /// Starts a coroutine remotely.
    /// </summary>
    /// <param name="coroutine"> The coroutine to start. </param>
    public static void StartCoroutine(this IEnumerator coroutine) => CoroutineServiceComponent.StartCoroutine(coroutine);

    /// <summary>
    /// Calls an action after a certain amount of time has been waited.
    /// </summary>
    /// <param name="time"> The time to wait before calling the action. </param>
    /// <param name="OnComplete"> The action to execute once the time has been waited. </param>
    public static void ExecuteAfterWait(float time, Action OnComplete) => _WaitForSeconds(time, OnComplete).StartCoroutine();

    /// <summary>
    /// Basic callback coroutine.
    /// </summary>
    /// <param name="time"> Time to wait before calling the action. </param>
    /// <param name="OnComplete"> Action to call once the time has been waited. </param>
    /// <returns> Time waited. </returns>
    private static IEnumerator _WaitForSeconds(float time, Action OnComplete)
    {
        yield return new WaitForSeconds(time);
        OnComplete();
    }
}
