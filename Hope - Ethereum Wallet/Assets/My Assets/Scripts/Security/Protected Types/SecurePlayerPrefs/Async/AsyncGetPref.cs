using Hope.Security.Encryption.DPAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncGetPref<T> : IAsyncPref
{

    private readonly string secureKey;
    private readonly string secureEntropy;
    private readonly Action<T> onValueReceived;

    public AsyncGetPref(string secureKey, string secureEntropy, Action<T> onValueReceived)
    {
        this.secureKey = secureKey;
        this.secureEntropy = secureEntropy;
        this.onValueReceived = onValueReceived;
    }

    public void PerformPrefAction()
    {
        string encryptedValue = PlayerPrefs.GetString(secureKey);



        onValueReceived?.Invoke(PlayerPrefs.GetString(secureKey).ConvertTo<T>());
    }

    private async Task DecryptValue(string encryptedValue, string secureEntropy)
    {
        await Task.Run(() => encryptedValue.Unprotect().DPDecrypt(secureEntropy)).ConfigureAwait(false);
    }
}