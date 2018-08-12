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
using Hope.Utils.Random;
using Hope.Utils.Random.Abstract;

// TODO
// Remove DisposableData and use Actions with the DataContainer/RefType instead
// Dispose of the DataContainer/RefType with a Zero method, null it out, and GC.Collect()
// Initialize the ProtectedType with an attribute that the caller must have to be able to decrypt and use the DataContainer/RefType

public class HOPETesting : MonoBehaviour
{
    private void Start()
    {
        SpeedTest("SHA1", "seed", 64, RandomBytes.SHA1.GetBytes);
        SpeedTest("SHA3", "seed", 64, RandomBytes.SHA3.GetBytes);
        SpeedTest("SHA256", "seed", 64, RandomBytes.SHA256.GetBytes);
        SpeedTest("SHA384", "seed", 64, RandomBytes.SHA384.GetBytes);
        SpeedTest("SHA512", "seed", 64, RandomBytes.SHA512.GetBytes);
        SpeedTest("Keccak", "seed", 64, RandomBytes.Keccak.GetBytes);
        SpeedTest("Blake2b", "seed", 64, RandomBytes.Blake2b.GetBytes);
        SpeedTest("Whirlpool", "seed", 64, RandomBytes.Whirlpool.GetBytes);
    }

    private void SpeedTest(string algoName, string seed, int length, Func<string, int, byte[]> getBytesFunc)
    {
        Stopwatch watch = Stopwatch.StartNew();

        for (int i = 0; i < 100000; i++)
            getBytesFunc.Invoke(seed, length);

        watch.Stop();
        Debug.Log(algoName + " => " + watch.ElapsedMilliseconds);
    }

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