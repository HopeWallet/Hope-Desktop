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
using System.Dynamic;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Reflection;
using UnityEditor;

public class Tester
{
    [ReflectionProtect]
    private void DoStuff()
    {
    }

    [ReflectionProtect(typeof(int))]
    private int DoMoreStuff()
    {
        return 0;
    }
}

public class HOPETesting : MonoBehaviour
{

    private const string CONTAINER_NAME = "MyContainer";
    private const int KEY_SIZE = 1024;

    private const string TEST_CLASS_STRING = @"public class Tester
{
    [ReflectionProtect]
    private void DoStuff()
    {
    }

    [ReflectionProtect(typeof(int))]
    private int DoMoreStuff()
    {
        return 0;
    }
}";

    private void Start()
    {
        var list = LoopThroughAssemblies();
        var dictionary = LoopThroughAssets().Where(p => list.Select(t => t.ToString()).Contains(p.Key));

        dictionary.ForEach(p => Debug.Log(p.Key + " => " + p.Value));
        list.ForEach(type => type.Log());
    }

    private Dictionary<string, string> LoopThroughAssets()
    {
        Dictionary<string, string> types = new Dictionary<string, string>();
        foreach (string assetPath in AssetDatabase.GetAllAssetPaths())
        {
            if (assetPath.EndsWith(".cs"))
            {
                string removedEnd = assetPath.Remove(assetPath.Length - 3, 3);
                string finalText = removedEnd.Remove(0, removedEnd.LastIndexOf('/') + 1);
                if (!types.ContainsKey(finalText))
                    types.Add(finalText, assetPath);
            }
        }
        return types;
    }

    private List<Type> LoopThroughAssemblies()
    {
        List<Type> types = new List<Type>();
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            types.AddItems(GetAttributeTypes<ReflectionProtectAttribute>(assembly).ToArray());
        }

        //types.ForEach(t => Debug.Log(t.ToString()));
        return types;
    }

    private List<Type> GetAttributeTypes<T>(Assembly assembly) where T : Attribute
    {
        List<Type> typesContainingAttributes = new List<Type>();
        foreach (Type type in assembly.GetTypes())
        {
            try
            {
                foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (Attribute.IsDefined(method, typeof(T)) && !typesContainingAttributes.Contains(type))
                        typesContainingAttributes.Add(type);
                }
            }
            catch (BadImageFormatException e)
            {
            }
        }

        return typesContainingAttributes;
    }

    //private void Start()
    //{
    //    //const string text = "this is my piece of text";

    //    //byte[] encrypted = Encrypt(text.GetUTF8Bytes());
    //    //byte[] decrypted = Decrypt(encrypted);

    //    //DeleteCspKeys();

    //    //ReflectionCall();

    //    //encrypted.GetBase64String().Log();
    //    //decrypted.GetUTF8String().Log();
    //}

    private void ReflectionCall()
    {
        Type type = this.GetType();
        MethodInfo methodInfo = type.GetMethod("DeleteCspKeys", BindingFlags.NonPublic | BindingFlags.Instance);
        methodInfo.Invoke(this, null);
    }

    [ReflectionProtect(typeof(HOPETesting))]
    private void DeleteCspKeys()
    {
        if (RuntimeMethodSearcher.FindReflectionCalls())
            Debug.Log("REFLECTION CALLED");

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