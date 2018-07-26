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
using Hope.Security.Encryption.Symmetric;


// TODO
// Remove DisposableData and use Actions with the DataContainer/RefType instead
// Dispose of the DataContainer/RefType with a Zero method, null it out, and GC.Collect()
// Initialize the ProtectedType with an attribute that the caller must have to be able to decrypt and use the DataContainer/RefType

public class HOPETesting : MonoBehaviour
{

    [Inject]
    private EthereumNetworkManager ethereumNetworkManager;

    private static int counter = 0;
    private const int ITR = 380;

    // individual unity = 286ms
    // individual regular = 514ms

    private void Start()
    {
    }

    [ContextMenu("Download String Unity")]
    public void DownloadStringUnity()
    {
        string url = ethereumNetworkManager.CurrentNetwork.Api.GetTokenBalanceUrl("0x5831819C84C05DdcD2568dE72963AC9f1e6831b6", "0xb332Feee826BF44a431Ea3d65819e31578f30446");
        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < ITR; i++)
        {
            UnityWebUtils.DownloadString(url, str =>
            {
                counter++;
                //Debug.Log("Done... #" + (counter) + " => " + str);
                if (counter == ITR - 1)
                {
                    counter = 0;
                    stopwatch.Stop();
                    Debug.Log(str + " => " + stopwatch.ElapsedMilliseconds);
                }
            });
        }
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