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

public class HOPETesting : MonoBehaviour
{

    public string entropy = "";

    private readonly EphemeralEncryption ephemeralEncrypt = new EphemeralEncryption();

    // TODO
    // DataContainer/RefType
    // private new int GetHashCode()
    // EphemeralEncrypt instance in each ProtectedType
    // Remove DisposableData and use Actions with the DataContainer/RefType instead
    // Dispose of the DataContainer/RefType with a Zero method, null it out, and GC.Collect()
    // Initialize the ProtectedType with an attribute that the caller must have to be able to decrypt and use the DataContainer/RefType

    [SecureCallEnd]
    private void Start()
    {
        const string text = "this is my piece of text";

        byte[] encrypted = ephemeralEncrypt.Encrypt(text.GetUTF8Bytes());
        byte[] decrypted = ephemeralEncrypt.Decrypt(encrypted);

        //DeleteCspKeys();

        //ReflectionCall();

        encrypted.GetBase64String().Log();
        decrypted.GetUTF8String().Log();
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