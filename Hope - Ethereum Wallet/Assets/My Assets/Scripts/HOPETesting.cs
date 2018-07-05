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
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Hope.Security.ProtectedTypes.Types.Base;
using System.ComponentModel;
using System.Numerics;
using Hope.Utils.EthereumUtils;
using System.Security;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Runtime.InteropServices;

public class HOPETesting : MonoBehaviour
{

    public PlayerPrefPassword prefPassword;
    public string[] words = new string[12];
    public int walletNum = 1;
    public string password;

    [Inject] private PopupManager popupManager;
    [Inject] private EthereumNetworkManager ethereumNetwork;
    [Inject] private ProtectedStringDataCache protectedStringDataCache;

    private UserWalletNew walletTest;

    private string lastPass = "013874013840183401384173048130487103847103784";

    private void Start()
    {
        //var ledger = LedgerClient.GetHIDLedgers().First();
        //var firmware = ledger.GetFirmwareVersion();
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("1'/0"));
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("44'/60'/0'/0'/0"));
        //Debug.Log(pubkey.Address);
        //Debug.Log(firmware);

        //const string mnemonic = "ridge capable pact idea interest fame okay nice rack rack surface rack";
        //Wallet wallet = new Wallet(mnemonic, null, WalletUtils.DetermineCorrectPath(mnemonic));

        //string seed = wallet.Seed.GetHexString();
        //byte[] byteSeed = seed.HexToByteArray();
        //Wallet newWallet = new Wallet(byteSeed);

        //newWallet.GetAddresses(20)[0].Log();

        walletTest = new UserWalletNew(prefPassword, popupManager, ethereumNetwork.CurrentNetwork, protectedStringDataCache);
    }

    private void Update()
    {
        if (lastPass != password)
        {
            protectedStringDataCache.SetData(new ProtectedString(password), 0);
            lastPass = password;
        }
    }

    [ContextMenu("Create Wallet")]
    public void CreateWallet()
    {
        walletTest.Create(string.Join(" ", words).Trim());
    }

    [ContextMenu("Unlock Wallet")]
    public void UnlockWallet()
    {
        walletTest.Unlock(walletNum);
    }

    [ContextMenu("Get Address")]
    public void GetAddress()
    {
        walletTest.GetAddress(0).Log();
    }

    private void DoStuff()
    {
        byte[] original = new byte[50];
        byte[] clear = new byte[original.Length];

        unsafe
        {
            fixed (byte* ptr = &original[0])
            {
                Marshal.Copy(clear, 0, new IntPtr(ptr), original.Length);
            }
        }
    }
}