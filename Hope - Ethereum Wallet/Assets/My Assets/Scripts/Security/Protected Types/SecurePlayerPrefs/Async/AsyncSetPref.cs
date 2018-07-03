using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncSetPref : IAsyncPref
{

    private readonly string key;
    private readonly string value;
    private readonly Action onPrefSet;

    public AsyncSetPref(string key, string value, Action onPrefSet)
    {
        this.key = key;
        this.value = value;
        this.onPrefSet = onPrefSet;
    }

    public void PerformPrefAction()
    {
        PlayerPrefs.SetString(key, value);
        onPrefSet?.Invoke();
    }
}