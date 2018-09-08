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
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
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

public sealed class HopeTesting : MonoBehaviour
{
    public bool getAddress;
    public bool signTransaction;

    private void Update()
    {
        if (getAddress)
        {
            getAddress = false;
            Task.Factory.StartNew(GetAddress);
        }
        else if (signTransaction)
        {
            signTransaction = false;
            Task.Factory.StartNew(async () =>
            {
                byte[] nonce = 0.ToBytesForRLPEncoding();
                byte[] gasPrice = 1000000000.ToBytesForRLPEncoding();
                byte[] gasLimit = 21000.ToBytesForRLPEncoding();
                byte[] address = "0x8b069Ecf7BF230E153b8Ed903bAbf244034cA203".HexToByteArray();
                byte[] value = SolidityUtils.ConvertToUInt(0.001m, 18).ToBytesForRLPEncoding();
                byte[] data = "0x".HexToByteArray();

                var response = await SignTransaction(CreateTransaction(nonce, gasPrice, gasLimit, address, value, data).GetRLPEncoded());
                var v = response.SignatureV;
                var r = response.SignatureR;
                var s = response.SignatureS;

                TransactionChainId transactionChainId = new TransactionChainId(nonce, gasPrice, gasLimit, address, value, data, new byte[] { 4 }, r, s, new byte[] { (byte)v });

                //v.Log();
                //r.LogArray();
                //s.LogArray();

                //var ecdsa = EthECDSASignatureFactory.FromComponents(r, s, (byte)v);
                //var signature = ecdsa.ToDER();

                //signature.LogArray();
                //signature.ToHex().Log();
                //Encoding.UTF8.GetString(signature).Log();

                EthSendRawTransactionUnityRequest ethSendRawTransaction = new EthSendRawTransactionUnityRequest(EthereumNetworkManager.Instance.CurrentNetwork.NetworkUrl);
                MainThreadExecutor.QueueAction(() => ethSendRawTransaction.SendRequest(transactionChainId.GetRLPEncoded().ToHex()).StartCoroutine());
            });
        }
    }

    private Transaction CreateTransaction(byte[] nonce, byte[] gasPrice, byte[] gasLimit, byte[] address, byte[] value, byte[] data)
    {
        return new Transaction(
            nonce,
            gasPrice,
            gasLimit,
            address,
            value,
            data,
            0.ToBytesForRLPEncoding(), // R
            0.ToBytesForRLPEncoding(), // S
            4);
    }

    private async Task<EthereumAppSignTransactionResponse> SignTransaction(byte[] rlpEncodedData)
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();

        if (ledgerManager == null)
            return null;

        ledgerManager.SetCoinNumber(60);

        var derivationData = Ledger.Net.Helpers.GetDerivationPathData(ledgerManager.CurrentCoin.App, ledgerManager.CurrentCoin.CoinNumber, 0, 2, false, ledgerManager.CurrentCoin.IsSegwit);
        var firstRequest = new EthereumAppSignTransactionRequest(derivationData.Concat(rlpEncodedData).ToArray());

        return await ledgerManager.SendRequestAsync<EthereumAppSignTransactionResponse, EthereumAppSignTransactionRequest>(firstRequest);
    }

    private static async Task GetAddress()
    {
        var ledgerManager = LedgerConnector.GetWindowsConnectedLedger();
        var address = await ledgerManager.GetAddressAsync(0, 2);
        Debug.Log(address);
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

    //private void AnonymousStuff()
    //{
    //    var thing = new { Name = "Something", Age = 50 };
    //    var things = new[] { new { Name = "Something1", Age = 25 }, new { Name = "Something2", Age = 35 } };

    //    dynamic obj = new ExpandoObject();
    //    obj.Stuff = new ExpandoObject[20];
    //    obj.Stuff[0].Something = "wow";
    //    obj.Name = "MyName";
    //    obj.Age = 22;

    //    Debug.Log(obj.Name);

    //}

}