﻿using LedgerWallet;
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

public class HOPETesting : MonoBehaviour
{

    public PlayerPrefPassword prefPassword;

    [Inject] private PopupManager popupManager;
    [Inject] private EthereumNetworkManager ethereumNetwork;
    [Inject] private ProtectedStringDataCache protectedStringDataCache;
    [Inject] private UpdateManager updateManager;

    private UserWalletNew walletTest;

    private void Start()
    {
        //var ledger = LedgerClient.GetHIDLedgers().First();
        //var firmware = ledger.GetFirmwareVersion();
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("1'/0"));
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("44'/60'/0'/0'/0"));
        //Debug.Log(pubkey.Address);
        //Debug.Log(firmware);

        walletTest = new UserWalletNew(prefPassword, popupManager, ethereumNetwork.CurrentNetwork, protectedStringDataCache, updateManager);
        protectedStringDataCache.SetData(new ProtectedString("testpassword"), 0);
        walletTest.Create("ridge capable pact idea interest fame okay nice trophy rack surface rack");
    }

}