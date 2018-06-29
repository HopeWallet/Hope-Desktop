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



        //PasswordHash passwordHash = new PasswordHash("this is a password");
        //byte[] hashBytes = passwordHash.ToArray();
        //byte[] hashBytes = Encoding.UTF8.GetBytes("13dfe3c3c4d463863c7b4a1541d1f6f3ebc0a20b82530c0f1c6d399d8c87b002d099b123");

        //PasswordHash verifyHash = new PasswordHash(hashBytes);
        //verifyHash.Verify("this is a password").Log();
        //hashBytes.ToHexString().Log();



        //BouncyCastleHashing mainHashingLib = new BouncyCastleHashing();

        //var password = "password"; // That's really secure! :)

        //byte[] saltBytes = mainHashingLib.CreateSalt();
        //string saltString = Convert.ToBase64String(saltBytes);

        //string pwdHash = mainHashingLib.PBKDF2_SHA256_GetHash(password, saltString);

        //string password2 = "password";

        //var isValid = mainHashingLib.ValidatePassword(password2, saltBytes, Convert.FromBase64String(pwdHash));
        ////pwdHash.Log();
        ////isValid.Log();
        //Convert.ToBase64String(saltBytes).Log();
        //pwdHash.Log();

        //string salt = "Ma0rWvAm7ydOr4G/++goNeb4VbNPrfgRvsDWih6koJ/vOfOJvswvu5Cqj5XByxqQodGCsN9o9T+e1sWZTOS0Ug==";
        //string hash = "4weScmLHeaZJgBHrs/wbe0c8vlY0fXDJXn6Kyj9F6MZyNtpsmgBfkinge1JRxDJ1iX+R/CfVQ828P2QFKRTfEFR/ZRwdISWAaRcZMqU23irifF+hbpmwmsxabQOUbFScqXw4z5ksi9Bnzz5pF19/iJ4ISPATbeiLuejomVbmGvQ=";

        //BouncyCastleHashing hashing = new BouncyCastleHashing();
        //hashing.ValidatePassword("password", Convert.FromBase64String(salt), Convert.FromBase64String(hash)).Log();

        string saltedHash = WalletPasswordEncryption.GetSaltedPasswordHash("password");
        WalletPasswordEncryption.VerifyPassword("password", saltedHash).Log();
        
    }

}