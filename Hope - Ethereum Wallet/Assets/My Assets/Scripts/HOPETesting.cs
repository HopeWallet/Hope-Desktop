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

public class HOPETesting : MonoBehaviour
{

    private const string CONTAINER_NAME = "MyContainer";
    private const int KEY_SIZE = 1024;

    //[ContextMenu("Start")]
    //public void Start()
    //{
    //    ReflectionProtectionInjector.Inject();
    //}

    //private void Start()
    //{
    //    const string text = "this is my piece of text";

    //    byte[] encrypted = Encrypt(text.GetUTF8Bytes());
    //    byte[] decrypted = Decrypt(encrypted);

    //    //DeleteCspKeys();

    //    ReflectionCall();

    //    //encrypted.GetBase64String().Log();
    //    //decrypted.GetUTF8String().Log();
    //}

    private void ReflectionCall()
    {
        Type type = this.GetType();
        FieldInfo fieldInfo = type.GetField("KEY_SIZE", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        MethodInfo methodInfo = type.GetMethod("DeleteCspKeys", BindingFlags.NonPublic | BindingFlags.Instance);
        fieldInfo.GetValue(this).ToString().Log();
        methodInfo.Invoke(this, null);
    }

    protected void DeleteCspKeys()
    {
        var rsa = GetRSA();
        rsa.PersistKeyInCsp = false;
        rsa.Clear();
    }

    private byte[] Encrypt(byte[] plain)
    {
        using (var rsa = GetRSA())
            return rsa.Encrypt(plain, true);
    }

    private byte[] Decrypt(byte[] encrypted)
    {
        using (var rsa = GetRSA())
            return rsa.Decrypt(encrypted, true);
    }

    private RSACryptoServiceProvider GetRSA()
    {
        CspParameters cspParameters = new CspParameters(1, null, CONTAINER_NAME)
        {
            Flags = CspProviderFlags.UseUserProtectedKey
        };

        return new RSACryptoServiceProvider(KEY_SIZE, cspParameters);
    }

    //[ContextMenu("Get Hash")]
    public string GetEncryptionHash()
    {
        var process = Process.GetCurrentProcess();

        var idHash = process.Id.ToString().GetSHA384Hash();
        var moduleHash = process.MainModule.ModuleName.GetHashCode().ToString().GetSHA256Hash();
        var instanceHash = RuntimeHelpers.GetHashCode(this).ToString().GetSHA384Hash();

        return idHash.CombineAndRandomize(moduleHash).GetSHA384Hash().CombineAndRandomize(instanceHash).GetSHA512Hash();
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