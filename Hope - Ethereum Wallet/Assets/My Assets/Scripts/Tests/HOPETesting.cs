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
using Hope.Utils.EthereumUtils;
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

// TODO
// Remove DisposableData and use Actions with the DataContainer/RefType instead
// Dispose of the DataContainer/RefType with a Zero method, null it out, and GC.Collect()
// Initialize the ProtectedType with an attribute that the caller must have to be able to decrypt and use the DataContainer/RefType

public class HOPETesting : MonoBehaviour
{
    private readonly Test test = new Test();
    private string encryptedData = "AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAA4HJ/V+iGNEebdnn7A/tc9QAAAAACAAAAAAAQZgAAAAEAACAAAABMbg6SZjqYtdMVfrmGKojIq90RBOfEol8t5sWbLvQF6AAAAAAOgAAAAAIAACAAAACRdzMBgEydeHR0D2vSEjmt6quoWtaWaRZyExzuw377MCAAAADoijlHIMxnt5oDyg6MQ/VgemvYPjSdKMBr4f5kJFnJzkAAAABHnhtCaqRkXQf2i5if+jMvMgY/5WNRLhf9hcec9ve2x4mOYUNuW31WU6xt52bX/EGOoLfgl3t/Syra2PKE86T1";

    [SecureCallEnd]
    private void Start()
    {
        //encryptedData = GetProtector().Protect("this is some nice text");
    }

    [ContextMenu("Secure")]
    [SecureCallEnd]
    public void SecureDecrypt()
    {
        Debug.Log(encryptedData);
        Debug.Log(GetProtector().Unprotect(encryptedData));
    }

    [ContextMenu("Insecure")]
    public void InsecureDecrypt()
    {
        Debug.Log(encryptedData);
        Debug.Log(GetProtector().Unprotect(encryptedData, "try me"));
    }

    [SecureCaller]
    private HopeDataProtector GetProtector() => new HopeDataProtector(1, 5, 4, 18, "nice one", false);

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

public class Test : SecureObject
{
}