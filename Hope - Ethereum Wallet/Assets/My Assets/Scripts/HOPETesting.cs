using LedgerWallet;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Web3.Accounts;
using System.Linq;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class HOPETesting : MonoBehaviour
{

    private void Start()
    {
        //var ledger = LedgerClient.GetHIDLedgers().First();
        //var firmware = ledger.GetFirmwareVersion();
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("1'/0"));
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("44'/60'/0'/0'/0"));
        //Debug.Log(pubkey.Address);
        //Debug.Log(firmware);
        string test = "Hello this is a test";
        byte[] testBytes = Encoding.UTF8.GetBytes(test);
        Encoding.UTF8.GetString(testBytes).Log();
    }

    private void Update()
    {   
    }

}