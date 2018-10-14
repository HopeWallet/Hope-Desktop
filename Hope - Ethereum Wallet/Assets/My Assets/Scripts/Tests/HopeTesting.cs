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

public sealed class HopeTesting : MonoBehaviour
{
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

//public static class Utils
//{
//    internal static byte[] SafeSubarray(this byte[] array, int offset, int count)
//    {
//        if (array == null)
//            throw new ArgumentNullException(nameof(array));
//        if (offset < 0 || offset > array.Length)
//            throw new ArgumentOutOfRangeException("offset");
//        if (count < 0 || offset + count > array.Length)
//            throw new ArgumentOutOfRangeException("count");
//        if (offset == 0 && array.Length == count)
//            return array;
//        var data = new byte[count];
//        Buffer.BlockCopy(array, offset, data, 0, count);
//        return data;
//    }

//    internal static byte[] SafeSubarray(this byte[] array, int offset)
//    {
//        if (array == null)
//            throw new ArgumentNullException(nameof(array));
//        if (offset < 0 || offset > array.Length)
//            throw new ArgumentOutOfRangeException("offset");

//        var count = array.Length - offset;
//        var data = new byte[count];
//        Buffer.BlockCopy(array, offset, data, 0, count);
//        return data;
//    }
//}

///// <summary>
///// A public HD key
///// </summary>
//public class ExtPubKey : IBitcoinSerializable, IDestination
//{
//    public static ExtPubKey Parse(string wif, NBitcoin.Network expectedNetwork = null)
//    {
//        return null;
//        //return NBitcoin.Network.Parse<BitcoinExtPubKey>(wif, expectedNetwork).ExtPubKey;
//    }

//    private const int FingerprintLength = 4;
//    private const int ChainCodeLength = 32;

//    static readonly byte[] validPubKey = Encoders.Hex.DecodeData("0374ef3990e387b5a2992797f14c031a64efd80e5cb843d7c1d4a0274a9bc75e55");
//    internal byte nDepth;
//    internal byte[] vchFingerprint = new byte[FingerprintLength];
//    internal uint nChild;

//    internal PubKey pubkey = new PubKey(validPubKey);
//    internal byte[] vchChainCode = new byte[ChainCodeLength];

//    public byte Depth
//    {
//        get
//        {
//            return nDepth;
//        }
//    }

//    public uint Child
//    {
//        get
//        {
//            return nChild;
//        }
//    }

//    public bool IsHardened
//    {
//        get
//        {
//            return (nChild & 0x80000000u) != 0;
//        }
//    }
//    public PubKey PubKey
//    {
//        get
//        {
//            return pubkey;
//        }
//    }
//    public byte[] ChainCode
//    {
//        get
//        {
//            byte[] chainCodeCopy = new byte[ChainCodeLength];
//            Buffer.BlockCopy(vchChainCode, 0, chainCodeCopy, 0, ChainCodeLength);

//            return chainCodeCopy;
//        }
//    }

//    internal ExtPubKey()
//    {
//    }

//    /// <summary>
//    /// Constructor. Creates a new extended public key from the specified extended public key bytes.
//    /// </summary>
//    public ExtPubKey(byte[] bytes)
//    {
//        if (bytes == null)
//            throw new ArgumentNullException(nameof(bytes));
//        this.ReadWrite(bytes);
//    }

//    /// <summary>
//    /// Constructor. Creates a new extended public key from the specified extended public key bytes, from the given hex string.
//    /// </summary>
//    public ExtPubKey(string hex)
//        : this(Encoders.Hex.DecodeData(hex))
//    {
//    }

//    public ExtPubKey(PubKey pubkey, byte[] chainCode, byte depth, byte[] fingerprint, uint child)
//    {
//        if (pubkey == null)
//            throw new ArgumentNullException(nameof(pubkey));
//        if (chainCode == null)
//            throw new ArgumentNullException(nameof(chainCode));
//        if (fingerprint == null)
//            throw new ArgumentNullException(nameof(fingerprint));
//        if (fingerprint.Length != FingerprintLength)
//            throw new ArgumentException(string.Format("The fingerprint must be {0} bytes.", FingerprintLength), "fingerprint");
//        if (chainCode.Length != ChainCodeLength)
//            throw new ArgumentException(string.Format("The chain code must be {0} bytes.", ChainCodeLength), "chainCode");
//        this.pubkey = pubkey;
//        this.nDepth = depth;
//        this.nChild = child;
//        Buffer.BlockCopy(fingerprint, 0, vchFingerprint, 0, FingerprintLength);
//        Buffer.BlockCopy(chainCode, 0, vchChainCode, 0, ChainCodeLength);
//    }

//    public ExtPubKey(PubKey masterKey, byte[] chainCode)
//    {
//        if (masterKey == null)
//            throw new ArgumentNullException(nameof(masterKey));
//        if (chainCode == null)
//            throw new ArgumentNullException(nameof(chainCode));
//        if (chainCode.Length != ChainCodeLength)
//            throw new ArgumentException(string.Format("The chain code must be {0} bytes.", ChainCodeLength), "chainCode");
//        this.pubkey = masterKey;
//        Buffer.BlockCopy(chainCode, 0, vchChainCode, 0, ChainCodeLength);
//    }


//    public bool IsChildOf(ExtPubKey parentKey)
//    {
//        if (Depth != parentKey.Depth + 1)
//            return false;
//        return parentKey.CalculateChildFingerprint().SequenceEqual(Fingerprint);
//    }
//    public bool IsParentOf(ExtPubKey childKey)
//    {
//        return childKey.IsChildOf(this);
//    }
//    public byte[] CalculateChildFingerprint()
//    {
//        return pubkey.Hash.ToBytes().SafeSubarray(0, FingerprintLength);
//    }

//    public byte[] Fingerprint
//    {
//        get
//        {
//            return vchFingerprint;
//        }
//    }

//    public ExtPubKey Derive(uint index)
//    {
//        var result = new ExtPubKey
//        {
//            nDepth = (byte)(nDepth + 1),
//            vchFingerprint = CalculateChildFingerprint(),
//            nChild = index
//        };
//        result.pubkey = pubkey.Derivate(this.vchChainCode, index, out result.vchChainCode);
//        return result;
//    }

//    public ExtPubKey Derive(KeyPath derivation)
//    {
//        ExtPubKey result = this;
//        return derivation.Indexes.Aggregate(result, (current, index) => current.Derive(index));
//    }

//    public ExtPubKey Derive(int index, bool hardened)
//    {
//        if (index < 0)
//            throw new ArgumentOutOfRangeException("index", "the index can't be negative");
//        uint realIndex = (uint)index;
//        realIndex = hardened ? realIndex | 0x80000000u : realIndex;
//        return Derive(realIndex);
//    }

//    public BitcoinExtPubKey GetWif(NBitcoin.Network network)
//    {
//        return null;
//        //return new BitcoinExtPubKey(this, network);
//    }

//    #region IBitcoinSerializable Members

//    public void ReadWrite(BitcoinStream stream)
//    {
//        using (stream.BigEndianScope())
//        {
//            stream.ReadWrite(ref nDepth);
//            stream.ReadWrite(ref vchFingerprint);
//            stream.ReadWrite(ref nChild);
//            stream.ReadWrite(ref vchChainCode);
//            stream.ReadWrite(ref pubkey);
//        }
//    }


//    private uint256 Hash
//    {
//        get
//        {
//            return Hashes.Hash256(this.ToBytes());
//        }
//    }

//    public override bool Equals(object obj)
//    {
//        ExtPubKey item = obj as ExtPubKey;
//        if (item == null)
//            return false;
//        return Hash.Equals(item.Hash);
//    }
//    public static bool operator ==(ExtPubKey a, ExtPubKey b)
//    {
//        if (System.Object.ReferenceEquals(a, b))
//            return true;
//        if (((object)a == null) || ((object)b == null))
//            return false;
//        return a.Hash == b.Hash;
//    }

//    public static bool operator !=(ExtPubKey a, ExtPubKey b)
//    {
//        return !(a == b);
//    }

//    public override int GetHashCode()
//    {
//        return Hash.GetHashCode();
//    }
//    #endregion

//    public string ToString(NBitcoin.Network network)
//    {
//        return "";
//        //return new BitcoinExtPubKey(this, network).ToString();
//    }

//    #region IDestination Members

//    /// <summary>
//    /// The P2PKH payment script
//    /// </summary>
//    public Script ScriptPubKey
//    {
//        get
//        {
//            return PubKey.Hash.ScriptPubKey;
//        }
//    }

//    #endregion
//}