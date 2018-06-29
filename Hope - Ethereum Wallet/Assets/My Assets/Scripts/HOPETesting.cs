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
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Isopoh.Cryptography.Argon2;
using Org.BouncyCastle.Crypto.Digests;
using System.IO;
using System.Collections.Generic;

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
        //PlayerPrefs.DeleteAll();

        string password = "pass";
        byte[] byteData = ProtectedData.Protect(Convert.FromBase64String(password), null, DataProtectionScope.CurrentUser);

        byte[] encryptedBytes = byteData.PadData();
        ProtectedMemory.Protect(encryptedBytes, MemoryProtectionScope.SameProcess);

        ProtectedMemory.Unprotect(encryptedBytes, MemoryProtectionScope.SameProcess);
        byte[] decryptedData = ProtectedData.Unprotect(encryptedBytes.UnpadData(), null, DataProtectionScope.CurrentUser);
        Convert.ToBase64String(decryptedData).Log();
    }

}