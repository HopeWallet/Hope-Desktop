using LedgerWallet;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Web3.Accounts;
using System.Linq;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using Random = System.Random;
using SecureRandom = Org.BouncyCastle.Security.SecureRandom;
using Zenject;
using Hope.Security.Encryption;
using Hope.Security;

public class HOPETesting : MonoBehaviour
{

    private string value;

    public bool async = true;

    private void Start()
    {
        //var ledger = LedgerClient.GetHIDLedgers().First();
        //var firmware = ledger.GetFirmwareVersion();
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("1'/0"));
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("44'/60'/0'/0'/0"));
        //Debug.Log(pubkey.Address);
        //Debug.Log(firmware);
        SecurePlayerPrefs.SetString("value", "key");
        //value = SecurePlayerPrefs.GetStringAsync("value");
    }

    private void Update()
    {
        if (async)
        {
            for (int i = 0; i < 16; i++)
                SecurePlayerPrefs.GetStringAsync("value", null)/*.Log()*/;
        }
        else
        {
            for (int i = 0; i < 16; i++)
                SecurePlayerPrefs.GetString("value")/*.Log()*/;
        }
    }

}