using LedgerWallet;
using NBitcoin;
using System.Linq;
using UnityEngine;

public class HOPETesting : MonoBehaviour
{

    private void Start()
    {
        var ledger = LedgerClient.GetHIDLedgers().First();
        var firmware = ledger.GetFirmwareVersion();
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("1'/0"));
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("44'/60'/0'/0'/0"));
        //Debug.Log(pubkey.Address);
        Debug.Log(firmware);
    }

    private void Update()
    {   
    }

}