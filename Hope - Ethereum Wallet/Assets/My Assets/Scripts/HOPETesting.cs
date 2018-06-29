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



        //PasswordHash passwordHash = new PasswordHash("this is a password");
        //byte[] hashBytes = passwordHash.ToArray();
        //byte[] hashBytes = Encoding.UTF8.GetBytes("13dfe3c3c4d463863c7b4a1541d1f6f3ebc0a20b82530c0f1c6d399d8c87b002d099b123");

        //PasswordHash verifyHash = new PasswordHash(hashBytes);
        //verifyHash.Verify("this is a password").Log();
        //hashBytes.ToHexString().Log();



        //string password = "password";
        //string saltedHash = WalletPasswordEncryption.GetSaltedPasswordHash(password);
        //WalletPasswordEncryption.VerifyPassword("password", saltedHash).Log();

        //password.GetSHA512Hash().Log();

        SecureRandom secureRandom = new SecureRandom();
        byte[] key = SecureRandom.GetNextBytes(secureRandom, 16);
        byte[] iv = SecureRandom.GetNextBytes(secureRandom, 16);

        string password = "pass";
        byte[] byteData = ProtectedData.Protect(Convert.FromBase64String(password), null, DataProtectionScope.CurrentUser);

        byte[] encryptedBytes = Encrypt(Convert.ToBase64String(byteData), key, iv); Convert.ToBase64String(encryptedBytes).Log();
        ProtectedMemory.Protect(encryptedBytes, MemoryProtectionScope.SameProcess); Convert.ToBase64String(encryptedBytes).Log();

        ProtectedMemory.Unprotect(encryptedBytes, MemoryProtectionScope.SameProcess);
        string decryptedPass = Decrypt(Convert.ToBase64String(encryptedBytes), key, iv);

        byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(decryptedPass), null, DataProtectionScope.CurrentUser);
        Convert.ToBase64String(decryptedData).Log();
    }

    private byte[] Encrypt(string password, byte[] key, byte[] iv)
    {
        using (Aes aes = Aes.Create())
        {
            ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                    sw.Write(password);

                return ms.ToArray();
            }
        }
    }

    private string Decrypt(string password, byte[] key, byte[] iv)
    {
        using (Aes aes = Aes.Create())
        {
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(password)))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sw = new StreamReader(cs))
                return sw.ReadToEnd();
        }
    }

}