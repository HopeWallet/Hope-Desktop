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
    private SecurePlayerPrefList<TokenInfoJson> tokenList;

    private void Start()
    {
        tokenList = new SecurePlayerPrefList<TokenInfoJson>("tkns");
        var prps = new TokenInfoJson("0x5831819C84C05DdcD2568dE72963AC9f1e6831b6", "Purpose", "PRPS", 18);
        var gold = new TokenInfoJson("0x904a34858B7A8714B2459309A004A96F8E092cB2", "Gold", "GLD2", 0);
        var raeon = new TokenInfoJson("0xf518e67B86C7a201484c6B50B3Bed092D21Ccef7", "Raeon", "RAEON", 13);
        var dubi = new TokenInfoJson("0x8b069Ecf7BF230E153b8Ed903bAbf24403ccA203", "Decentralized Universal Basic Income", "DUBI", 18);

        //tokenList.IndexOf("0x8b069Ecf7BF230E153b8Ed903bAbf24403ccA203").Log();
        //tokenList.Remove("0x8b069Ecf7BF230E153b8Ed903bAbf24403ccA203").Log();
        tokenList.Insert(1, dubi);
    }

    [ContextMenu("Delete Player Prefs")]
    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
    }

	[ContextMenu("Delete Saved Contacts")]
	public void DeleteContacts()
	{
		for (int i = 1; i <= SecurePlayerPrefs.GetInt("Contacts"); i++)
		{
			SecurePlayerPrefs.DeleteKey(SecurePlayerPrefs.GetString("contact_" + i));
			SecurePlayerPrefs.DeleteKey("contact_" + i);
		}

		SecurePlayerPrefs.DeleteKey("Contacts");
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