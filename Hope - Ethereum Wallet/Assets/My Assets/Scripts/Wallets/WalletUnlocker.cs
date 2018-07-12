using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Threading.Tasks;

public class WalletUnlocker : WalletLoaderBase
{

    protected override string LoadingText => "Unlocking wallet...";

    public WalletUnlocker(
        PopupManager popupManager,
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache protectedStringDataCache) : base(popupManager, playerPrefPassword, protectedStringDataCache)
    {
    }

    protected override void LoadWallet(string userPass)
    {
        int walletNum = (int)dynamicDataCache.GetData("walletnum");

        if (!SecurePlayerPrefs.HasKey("password_" + walletNum))
        {
            ExceptionManager.DisplayException(new Exception("No wallet found with that number. Please try a different wallet."));
        }

        else
        {
            string saltedHash = SecurePlayerPrefs.GetString("password_" + walletNum);
            AsyncTaskScheduler.Schedule(() => TryPassword(walletNum, userPass, saltedHash));
        }
    }

    private async Task TryPassword(int walletNum, string password, string saltedHash)
    {
        // Experiment with potentially multiple layers of passwords
        // Layer 1 can be maybe 1/4th the password, and will have a very simplified version of the salted hash with less iterations.
        // Layer 2 can be maybe 1/2 the password, with more iterations than layer 1, but still less than the full password.
        // Checking these layers first will speed up the password checking process quite significantly.
        bool correctPassword = string.IsNullOrEmpty(password) ? false : await Task.Run(() => PasswordEncryption.VerifyPassword(password, saltedHash)).ConfigureAwait(false);

        if (!correctPassword)
            IncorrectPassword();
        else
            CorrectPassword(walletNum, password);
    }

    private void IncorrectPassword()
    {
        MainThreadExecutor.QueueAction(() => ExceptionManager.DisplayException(new Exception("Unable to unlock wallet, incorrect password. ")));
    }

    private void CorrectPassword(int walletNum, string password)
    {
        MainThreadExecutor.QueueAction(() =>
        {
            playerPrefPassword.PopulatePrefDictionary(walletNum);

            string[] hashLvls = new string[4];
            for (int i = 0; i < hashLvls.Length; i++)
                hashLvls[i] = SecurePlayerPrefs.GetString("wallet_" + walletNum + "_h" + (i + 1));

            AsyncTaskScheduler.Schedule(() => UnlockWalletAsync(hashLvls, SecurePlayerPrefs.GetString("wallet_" + walletNum), password));
        });
    }

    private async Task UnlockWalletAsync(string[] hashLvls, string encryptedSeed, string password)
    {
        var encryptionPassword = await Task.Run(() => playerPrefPassword.ExtractEncryptionPassword(password).GetSHA256Hash()).ConfigureAwait(false);
        var splitPass = encryptionPassword.SplitHalf();
        var lvl12string = splitPass.Item1.SplitHalf();
        var lvl34string = splitPass.Item2.SplitHalf();

        string unprotectedSeed = await Task.Run(() => encryptedSeed.Unprotect(hashLvls[2].Unprotect() + hashLvls[3].AESDecrypt(lvl34string.Item2.GetSHA512Hash()))).ConfigureAwait(false);
        byte[] decryptedSeed = await Task.Run(() => unprotectedSeed.AESDecrypt(hashLvls[0].AESDecrypt(lvl12string.Item1.GetSHA512Hash()) + hashLvls[1].Unprotect()).HexToByteArray()).ConfigureAwait(false);

        await GetAddresses(new Wallet(decryptedSeed)).ConfigureAwait(false);

        MainThreadExecutor.QueueAction(onWalletLoaded);
    }
}