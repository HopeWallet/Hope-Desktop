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
using System.Net;
using Trezor.Net.Contracts.Ethereum;
using Trezor.Net.Contracts.Bitcoin;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class HopeTesting : MonoBehaviour
{
    public Image image;

    [Serializable]
    public sealed class Token
    {
        public string address;
        public string name;
        public string symbol;
        public int? decimals;

        //public TokenImage tokenImage;
    }

    [Serializable]
    public sealed class TokenImage
    {
        public int width;
        public int height;
        public TextureFormat textureFormat;
        public string rawData;
    }

    public TextAsset json;
    public TextAsset newTextAsset;

    private readonly List<Token> tokens = new List<Token>();

    private int counter = 0;

    [Inject]
    private TradableAssetImageManager tradableAssetImageManager;

#if UNITY_EDITOR
    [ContextMenu("SAVE")]
    public void Save()
    {
        File.WriteAllText(Application.dataPath + "/tokens.txt", JsonUtils.Serialize(tokens));
        AssetDatabase.Refresh();
    }
#endif

    [ContextMenu("Load")]
    public void LoadData()
    {
        var text = newTextAsset.text;
        var jArray = JsonUtils.DeserializeDynamicCollection(text);

        var tokenImage = jArray[0].tokenImage;

        var rawData = ((string)tokenImage.rawData).GetBase64Bytes();
        Texture2D texture = new Texture2D((int)tokenImage.width, (int)tokenImage.height, (TextureFormat)tokenImage.textureFormat, false);
        texture.LoadRawTextureData(rawData);
        texture.Apply();

        image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new UnityEngine.Vector2(0.5f, 0.5f));
    }

    private void Start()
    {
        //var text = json.text;
        //var jArray = JsonUtils.DeserializeDynamicCollection(text);

        //for (int i = 0; i < jArray.Count; i++)
        //    Scan((string)jArray[i].address);

        //tradableAssetImageManager.LoadImage("PRPS", img =>
        //{
        //    //img.texture.GetRawTextureData().GetBase64String().Log();

        //    img.texture.width.Log();
        //    img.texture.height.Log();
        //    img.texture.format.Log();

        //    File.WriteAllText(Application.dataPath + "/prps.txt", img.texture.GetRawTextureData().GetBase64String());
        //    AssetDatabase.Refresh();

        //    //Texture2D texture = new Texture2D(img.texture.width, img.texture.height, img.texture.format, false);
        //    //texture.LoadRawTextureData(img.texture.GetRawTextureData().GetBase64String().GetBase64Bytes());
        //    //texture.Apply();

        //    //image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new UnityEngine.Vector2(0.5f, 0.5f));
        //});
        //tradableAssetImageManager.LoadImage("SFDOHSDFOUHSUOH", img =>
        //{
        //    defaultImage = img;
        //    //Scan("0xa4e8c3ec456107ea67d3075bf9e3df3a75823db0");
        //    //Scan("0x9f8F72aA9304c8B593d555F12eF6589cC3A579A2");

        //var text = json.text;
        //var jArray = JsonUtils.DeserializeDynamicCollection(text);

        //for (int i = 0; i < jArray.Count; i++)
        //    Scan((string)jArray[i].address);
        //});
    }

    public byte[] FromHex(string hex)
    {
        hex = hex.Replace("-", "");
        byte[] raw = new byte[hex.Length / 2];
        for (int i = 0; i < raw.Length; i++)
        {
            raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return raw;
    }

    private void Scan(string address)
    {
        Token token = new Token { address = address };
        SimpleContractQueries.QueryStringOutput<ERC20.Queries.Name>(address, null).OnSuccess(name =>
        {
            if (string.IsNullOrEmpty(token.symbol) && !string.IsNullOrEmpty(name.Value))
                token.symbol = name.Value;
            if (!string.IsNullOrEmpty(name.Value))
                token.name = name.Value;
            UpdateToken(token);
        });
        SimpleContractQueries.QueryStringOutput<ERC20.Queries.Symbol>(address, null).OnSuccess(symbol =>
        {
            if (string.IsNullOrEmpty(token.name) && !string.IsNullOrEmpty(symbol.Value))
                token.name = symbol.Value;
            if (!string.IsNullOrEmpty(symbol.Value))
                token.symbol = symbol.Value;
            UpdateToken(token);
        });
        SimpleContractQueries.QueryUInt256Output<ERC20.Queries.Decimals>(address, null).OnSuccess(decimals =>
        {
            token.decimals = (int?)decimals.Value;
            UpdateToken(token);
        });
        SimpleContractQueries.QueryBytesOutput<ERC20.Queries.Name>(address, null).OnSuccess(name =>
        {
            string strName = null;
            if (!IsEmpty(name.Value))
            {
                strName = Encoding.ASCII.GetString(FromHex(name.Value.ToHex().TrimEnd('0')));
                if (string.IsNullOrEmpty(token.symbol) && !string.IsNullOrEmpty(strName))
                    token.symbol = strName;
                if (!string.IsNullOrEmpty(strName))
                    token.name = strName;
            }
            UpdateToken(token);
        });
        SimpleContractQueries.QueryBytesOutput<ERC20.Queries.Symbol>(address, null).OnSuccess(symbol =>
        {
            string strSymbol = null;
            if (!IsEmpty(symbol.Value))
            {
                strSymbol = Encoding.ASCII.GetString(FromHex(symbol.Value.ToHex().TrimEnd('0')));
                if (string.IsNullOrEmpty(token.name) && !string.IsNullOrEmpty(strSymbol))
                    token.name = strSymbol;
                if (!string.IsNullOrEmpty(strSymbol))
                    token.symbol = strSymbol;
            }

            UpdateToken(token);
        });
    }

    private bool IsEmpty(byte[] array)
    {
        if (array == null)
            return true;

        for (int i = 0; i < array.Length - 1; i++)
        {
            if (array[i] != 0)
                return false;
        }

        return true;
    }

    private void UpdateToken(Token token)
    {
        if (!string.IsNullOrEmpty(token.symbol) && !string.IsNullOrEmpty(token.name) && token.decimals.HasValue)
        {
            //if (token.tokenImage == null)
            //{
            //    tradableAssetImageManager.LoadImage(token.symbol, img =>
            //    {
            //        if (img != defaultImage)
            //        {
            //            token.tokenImage = new TokenImage { width = img.texture.width, height = img.texture.height, textureFormat = img.texture.format, rawData = img.texture.GetRawTextureData().GetBase64String() };
            //        }
            //    });
            //}

            if (tokens.Contains(token))
                return;

            tokens.Add(token);

            UnityEngine.Debug.Log(++counter);
        }
    }

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