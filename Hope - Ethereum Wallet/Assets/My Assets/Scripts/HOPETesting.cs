using LedgerWallet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class HOPETesting : MonoBehaviour
{

    private void Start()
    {
        var ledger = LedgerClient.GetHIDLedgers().First();
        Debug.Log(ledger);
    }

    private void Update()
    {
        
    }

}