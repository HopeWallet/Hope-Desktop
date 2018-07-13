using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

/// <summary>
/// Class used for scraping data off of web apis.
/// </summary>
public static class WebClientUtils
{

    /// <summary>
    /// Gets the contract abi from the api url.
    /// </summary>
    /// <param name="apiUrl"> The url of the contract abi. </param>
    /// <param name="onAbiReceived"> Action to execute and pass the abi as a parameter once it has successfully been processed. </param>
    public static async void GetContractABI(string apiUrl, Action<string> onAbiReceived)
    {
        onAbiReceived?.Invoke(await DownloadString(apiUrl, (abi) =>
        {
            if (abi != null)
            {
                abi = abi.Replace("\"", "'").Replace("\\", "");
                abi = abi.Substring(abi.IndexOf("["), abi.LastIndexOf("'") - abi.IndexOf("["));
            }

            return abi;
        }));
    }

    /// <summary>
    /// Downloads a string from a url.
    /// </summary>
    /// <param name="url"> The url which contains the string data. </param>
    /// <param name="onStringReceived"> Action to call once the string has been received. </param>
    public static async void DownloadString(string url, Action<string> onStringReceived)
    {
        onStringReceived?.Invoke(await DownloadString(url).ConfigureAwait(false));
    }

    /// <summary>
    /// Downloads a string from a given url.
    /// </summary>
    /// <param name="url"> The url to download the string from. </param>
    /// <param name="modifyString"> Func to execute once the string has been received. Used to perform string operations on before returning the final result. </param>
    /// <returns> Returns a task which downloads and performs operations on a retrieved from a url. </returns>
    private static Task<string> DownloadString(string url, Func<string, string> modifyString = null)
    {
        return Task.Run(() =>
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

            string apiString = null;

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    apiString = webClient.DownloadString(url);
                }

                if (modifyString != null)
                    apiString = modifyString(apiString);
            }
            catch
            {
                apiString = null;
            }

            return apiString;
        });
    }

    /// <summary>
    /// Allows us to successfully access the WebClient without errors.
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
