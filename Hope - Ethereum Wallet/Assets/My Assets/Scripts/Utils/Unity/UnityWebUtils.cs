using System;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Class used for retrieving data from websites.
/// </summary>
public static class UnityWebUtils
{
	/// <summary>
	/// Downloads string data from a given url.
	/// </summary>
	/// <param name="url"> The url containing string data. </param>
	/// <param name="onStringDataReceived"> Action called once the string data has been received. </param>
	public static void DownloadString(string url, Action<string> onStringDataReceived)
    {
        SendUnityWebRequest(url, webRequest => onStringDataReceived?.Invoke(webRequest.downloadHandler.text)).StartCoroutine();
    }

    /// <summary>
    /// Sends a <see cref="UnityWebRequest"/> to a certain url to retrieve some data.
    /// </summary>
    /// <param name="url"> The url to send the <see cref="UnityWebRequest"/> to. </param>
    /// <param name="onDataReceived"> Action called with the web request once it has finished processing the web data. </param>
    /// <returns> Coroutine used to execute the <see cref="UnityWebRequest"/>. </returns>
    private static IEnumerator SendUnityWebRequest(string url, Action<UnityWebRequest> onDataReceived)
    {
        var webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        if (webRequest.isHttpError || webRequest.isNetworkError)
            UnityEngine.Debug.Log(webRequest.error);

        onDataReceived?.Invoke(webRequest);
    }
}