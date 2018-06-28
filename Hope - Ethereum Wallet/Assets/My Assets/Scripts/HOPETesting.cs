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

    private void Start()
    {
        //var ledger = LedgerClient.GetHIDLedgers().First();
        //var firmware = ledger.GetFirmwareVersion();
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("1'/0"));
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("44'/60'/0'/0'/0"));
        //Debug.Log(pubkey.Address);
        //Debug.Log(firmware);

        PlayerPrefs.DeleteAll();

        string test = PasswordUtils.GenerateRandomPassword();
        test.GetMd5Hash().Log();
        test.GetSha1Hash().Log();
        test.GetSha256Hash().Log();
        test.GetSha384Hash().Log();
        test.GetSha512Hash().Log();
    }

    private void Update()
    {
    }

}