using System;
using System.Collections;
using UnityEngine.Networking;

public static class UnityWebUtils
{
    public static void DownloadString(string url, Action<string> onStringDataReceived) => DownloadStringWebRequest(url, onStringDataReceived).StartCoroutine();

    private static IEnumerator DownloadStringWebRequest(string url, Action<string> onStringDataReceived)
    {
        yield return SendUnityWebRequest(url, webRequest =>
        {
            if (webRequest.isHttpError || webRequest.isNetworkError)
                UnityEngine.Debug.Log(webRequest.error);
            onStringDataReceived?.Invoke(webRequest.downloadHandler.text);
        });
    }

    private static IEnumerator SendUnityWebRequest(string url, Action<UnityWebRequest> onDataReceived)
    {
        var webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        onDataReceived?.Invoke(webRequest);
    }
}