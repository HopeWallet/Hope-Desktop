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

public class HOPETesting : MonoBehaviour
{

    public bool activateAsync;
    public bool activate;

    private void Start()
    {
        //var ledger = LedgerClient.GetHIDLedgers().First();
        //var firmware = ledger.GetFirmwareVersion();
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("1'/0"));
        //var pubkey = ledger.GetWalletPubKey(new KeyPath("44'/60'/0'/0'/0"));
        //Debug.Log(pubkey.Address);
        //Debug.Log(firmware);
        //PlayerPrefs.DeleteAll();
    }

    private void Update()
    {
        if (activate)
        {
            activate = false;
            var s1 = Stopwatch.StartNew();
            for (int i = 0; i < 160; i++)
            {
                string key = PasswordUtils.GenerateRandomPassword() + RandomUtils.GenerateRandomHexLetter();
                string value = PasswordUtils.GenerateFixedLengthPassword(16);
            }
            s1.Stop();
            UnityEngine.Debug.Log(s1.ElapsedMilliseconds);
        }

        if (activateAsync)
        {
            activateAsync = false;
            var s1 = Stopwatch.StartNew();
            for (int i = 0; i < 160; i++)
            {
                AsyncTaskScheduler.Schedule(GenerateRandomPref);
            }
            s1.Stop();
            UnityEngine.Debug.Log(s1.ElapsedMilliseconds);
        }
    }

    private async Task GenerateRandomPref()
    {
        string key = await Task.Run(() => PasswordUtils.GenerateFixedLengthPassword(16));
        string value = await Task.Run(() => PasswordUtils.GenerateRandomPassword()) + await Task.Run(() => RandomUtils.GenerateRandomHexLetter());
        SecurePlayerPrefsAsync.SetString(key, value, () => UnityEngine.Debug.Log("String set!"));
    }
}