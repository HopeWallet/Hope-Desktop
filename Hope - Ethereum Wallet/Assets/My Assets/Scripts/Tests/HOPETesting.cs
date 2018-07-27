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
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.Extensions;
using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;
using System.Collections;
using Nethereum.JsonRpc.UnityClient;


// TODO
// Remove DisposableData and use Actions with the DataContainer/RefType instead
// Dispose of the DataContainer/RefType with a Zero method, null it out, and GC.Collect()
// Initialize the ProtectedType with an attribute that the caller must have to be able to decrypt and use the DataContainer/RefType

public class HOPETesting : MonoBehaviour
{
    public string contractAddress;

    [ContextMenu("Query")]
    public void Query()
    {
        //Testing("0x132962ed7C55326217C390176E053d1a6d335B62", ethereumNetworkManager.CurrentNetwork.NetworkUrl, "0x9c6Fa42209169bCeA032e401188a6fc3e9C9f59c").StartCoroutine();

        //if (!AddressUtils.IsValidEthereumAddress(contractAddress))
        //    return;

        ContractUtils.QueryContract<ERC20.Functions.BalanceOf, SimpleOutputs.UInt256>(contractAddress,
                                                                                      "0x132962ed7C55326217C390176E053d1a6d335B62",
                                                                                      OnBalanceReceived,
                                                                                      "0x132962ed7C55326217C390176E053d1a6d335B62");

        ContractUtils.QueryContract<ERC20.Functions.Name, SimpleOutputs.String>(contractAddress,
                                                                                "0x132962ed7C55326217C390176E053d1a6d335B62",
                                                                                OnTextReceived);

        ContractUtils.QueryContract<ERC20.Functions.Symbol, SimpleOutputs.String>(contractAddress,
                                                                                  "0x132962ed7C55326217C390176E053d1a6d335B62",
                                                                                  OnTextReceived);
    }

    private void OnBalanceReceived(SimpleOutputs.UInt256 uintOutput) => Debug.Log(uintOutput.Value);

    private void OnTextReceived(SimpleOutputs.String stringOutput) => Debug.Log(stringOutput.Value);

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