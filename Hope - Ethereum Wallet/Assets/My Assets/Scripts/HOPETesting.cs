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

public class HOPETesting : MonoBehaviour
{
	private void Start()
	{
		//var ledger = LedgerClient.GetHIDLedgers().First();
		//var firmware = ledger.GetFirmwareVersion();
		//var pubkey = ledger.GetWalletPubKey(new KeyPath("1'/0"));
		//var pubkey = ledger.GetWalletPubKey(new KeyPath("44'/60'/0'/0'/0"));
		//Debug.Log(pubkey.Address);
		//Debug.Log(firmware);
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

        UnityEngine.Debug.Log(obj.Name);

    }

}