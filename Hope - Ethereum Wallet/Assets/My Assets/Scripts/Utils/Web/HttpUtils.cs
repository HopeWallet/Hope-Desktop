using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

/// <summary>
/// Class used for scraping data off of web apis.
/// </summary>
public static class HttpUtils
{
    /// <summary>
    /// Downloads a string from a url.
    /// </summary>
    /// <param name="url"> The url which contains the string data. </param>
    /// <param name="onStringReceived"> Action to call once the string has been received. </param>
    public static void DownloadString(string url, Action<string> onStringReceived)
    {
        Task.Factory.StartNew(async () =>
        {
            string downloadedString = await DownloadString(url).ConfigureAwait(false);
            MainThreadExecutor.QueueAction(() => onStringReceived?.Invoke(downloadedString));
        });
    }

    /// <summary>
    /// Downloads a string from a given url.
    /// </summary>
    /// <param name="url"> The url to download the string from. </param>
    /// <returns> Returns a task which downloads and performs operations on a retrieved from a url. </returns>
    private static async Task<string> DownloadString(string url)
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        string downloadedString = null;

        try
        {
            using (HttpClient client = new HttpClient())
                downloadedString = await client.GetStringAsync(url).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            ExceptionManager.DisplayException(e);
        }

        return downloadedString;
    }

    /// <summary>
    /// Allows us to successfully access and make requests with the HttpClient without errors.
    /// </summary>
    private static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;

        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    continue;
                }
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                bool chainIsValid = chain.Build((X509Certificate2)certificate);
                if (!chainIsValid)
                {
                    isOk = false;
                    break;
                }
            }
        }

        return isOk;
    }

}