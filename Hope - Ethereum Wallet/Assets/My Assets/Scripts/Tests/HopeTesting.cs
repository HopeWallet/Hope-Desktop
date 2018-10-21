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
using Org.BouncyCastle.Crypto.Digests;
using System.IO;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Hope.Security.ProtectedTypes.Types.Base;
using System.ComponentModel;
using System.Numerics;
using Hope.Utils.Ethereum;
using System.Security;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Runtime.InteropServices;
using System.Dynamic;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Reflection;
using System.Security.Permissions;
using Hope.Security.Encryption.Symmetric;
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.Extensions;
using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;
using System.Collections;
using Nethereum.JsonRpc.UnityClient;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Org.BouncyCastle.Crypto.Prng;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Signer;
using Hope.Random;
using Transaction = Nethereum.Signer.Transaction;
using Nethereum.RLP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Hope.Random.Strings;
using Hope.Random.Bytes;
using Org.BouncyCastle.Asn1.Cms;
using Nethereum.RPC.NonceServices;
using HidLibrary;
using Ledger.Net.Connectivity;
using Ledger.Net.Requests;
using Ledger.Net.Responses;
using Hid.Net.Unity;
using UniRx;
using Nethereum.ABI.JsonDeserialisation;
using System.Globalization;
using Ledger.Net;
using NBitcoin.DataEncoders;
using NBitcoin.Crypto;
using Org.BouncyCastle.Utilities;
using System.Net;
using Trezor.Net.Contracts.Ethereum;
using Trezor.Net.Contracts.Bitcoin;

public sealed class HopeTesting : MonoBehaviour
{
    public static HopeTesting Instance;

    public string pin;

    private void Awake()
    {
        Instance = this;
    }

    //private async void Start()
    //{
    //    Instance = this;

    //    //https://www.red-gate.com/simple-talk/dotnet/c-programming/calling-restful-apis-unity3d/
    //    //HttpWebRequest httpWebRequest = HttpWebRequest.Create("") as HttpWebRequest;

    //    var trezor = TrezorConnector.GetWindowsConnectedTrezor(EnterPin);

    //    if (trezor == null)
    //        return;

    //    //await GetEthereumAddress(trezor);
    //    //await GetPublicKey(trezor);
    //    await SignTransaction(trezor);
    //}

    private static async Task SignTransaction(Trezor.Net.TrezorManager trezor)
    {
        EthereumSignTx ethereumSignTx = new EthereumSignTx
        {
            Nonce = 0.ToBytesForRLPEncoding(),
            AddressNs = KeyPath.Parse("m/44'/60'/0'/0/0").Indexes,
            GasPrice = 1000000000.ToBytesForRLPEncoding(),
            GasLimit = 21000.ToBytesForRLPEncoding(),
            To = "689c56aef474df92d44a1b70850f808488f9769c".HexToByteArray(),
            Value = BigInteger.Parse("1000000000000000000").ToBytesForRLPEncoding(),
            DataInitialChunk = new byte[0],
            DataLength = 0,
            ChainId = 4
        };

        var transaction = await trezor.SendMessageAsync<EthereumTxRequest, EthereumSignTx>(ethereumSignTx);
    }

    private static async Task GetPublicKey(Trezor.Net.TrezorManager trezor)
    {
        GetPublicKey getPublicKey = new GetPublicKey
        {
            AddressNs = KeyPath.Parse("m/44'/60'/0'").Indexes,
            ShowDisplay = false
        };

        PublicKey publicKey = await trezor.SendMessageAsync<PublicKey, GetPublicKey>(getPublicKey);

        ExtPubKey extPubKey = new ExtPubKey(new PubKey(publicKey.Node.PublicKey).Compress(), publicKey.Node.ChainCode);
        new EthECKey(extPubKey.Derive(0).Derive(0).PubKey.ToBytes(), false).GetPublicAddress().ConvertToEthereumChecksumAddress().Log();
    }

    private static async Task GetEthereumAddress(Trezor.Net.TrezorManager trezor)
    {
        EthereumGetAddress ethereumGetAddress = new EthereumGetAddress
        {
            AddressNs = KeyPath.Parse("m/44'/60'/0'/0/0").Indexes,
            ShowDisplay = false
        };

        EthereumAddress address = await trezor.SendMessageAsync<EthereumAddress, EthereumGetAddress>(ethereumGetAddress);

        if (address == null)
        {
            Debug.Log("NULL");
            return;
        }

        address.Address.ToHex(true).Log();
    }

    private async Task<string> EnterPin()
    {
        while (pin.Length < 4)
        {
            await Task.Delay(100);
        }

        return pin;
    }

    //public string code;

    //private string previousCode;
    //private TwoFactorAuthenticator _2fa;
    //private SetupCode setupCode;

    //private void Start()
    //{
    //    string key = RandomString.Secure.SHA3.GetString("testPassword", 256).Keccak_128();
    //    key.Log();

    //    Debug.Log("==================================");

    //    _2fa = new TwoFactorAuthenticator();

    //    setupCode = _2fa.GenerateSetupCode("Hope Wallet", key, 256, 256);
    //    setupCode.Account.Log();
    //    setupCode.AccountSecretKey.Log();
    //    setupCode.ManualEntryKey.Log();
    //    setupCode.QrCodeSetupImageUrl.Log();
    //}

    //private void Update()
    //{
    //    if (code == previousCode)
    //        return;

    //    previousCode = code;

    //    TwoFactorAuthenticator authenticator = new TwoFactorAuthenticator();
    //    authenticator.ValidateTwoFactorPIN(RandomString.Secure.SHA3.GetString("testPassword", 256).Keccak_128(), code).Log();
    //}

    [ContextMenu("Delete Player Prefs")]
    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    private void AnonymousStuff()
    {
        var thing = new { Name = "Something", Age = 50 };
        var things = new[] { new { Name = "Something1", Age = 25 }, new { Name = "Something2", Age = 35 } };

        dynamic obj = new ExpandoObject();
        obj.Stuff = new ExpandoObject[20];
        obj.Stuff[0].Something = "wow";
        obj.Name = "MyName";
        obj.Age = 22;

        Debug.Log(obj.Name);

    }

}