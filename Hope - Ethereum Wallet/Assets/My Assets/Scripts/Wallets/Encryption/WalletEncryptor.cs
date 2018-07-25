using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Org.BouncyCastle.Security;
using System;
using System.Threading.Tasks;

public sealed class WalletEncryptor : SecureObject
{

    private readonly PlayerPrefPassword playerPrefPassword;
    private readonly DynamicDataCache dynamicDataCache;

    public WalletEncryptor(
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache)
    {
        this.playerPrefPassword = playerPrefPassword;
        this.dynamicDataCache = dynamicDataCache;
    }

    public void EncryptWallet(byte[] seed, string passwordBase, Action<string[], string, string> onWalletEncrypted)
    {
        AsyncTaskScheduler.Schedule(() => AsyncEncryptWallet(seed, passwordBase, onWalletEncrypted));
    }

    private async Task AsyncEncryptWallet(byte[] seed, string passwordBase, Action<string[], string, string> onWalletEncrypted)
    {
        SecureRandom secureRandom = new SecureRandom();

        string encryptionPassword = await Task.Run(() => playerPrefPassword.GenerateEncryptionPassword(passwordBase).GetSHA256Hash()).ConfigureAwait(false);

        Tuple<string, string> splitPass = encryptionPassword.SplitHalf();
        Tuple<string, string> lvl12string = splitPass.Item1.SplitHalf();
        Tuple<string, string> lvl34string = splitPass.Item2.SplitHalf();

        string h1 = await Task.Run(() => lvl12string.Item1.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string h2 = await Task.Run(() => lvl12string.Item2.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string h3 = await Task.Run(() => lvl34string.Item1.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);
        string h4 = await Task.Run(() => lvl34string.Item2.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false);

        string encryptedSeed = await Task.Run(() => seed.GetHexString().AESEncrypt(h1 + h2).Protect(h3 + h4)).ConfigureAwait(false);
        string saltedPasswordHash = await Task.Run(() => PasswordEncryption.GetSaltedPasswordHash(passwordBase)).ConfigureAwait(false);

        string[] encryptedHashLvls = await Task.Run(() => new string[] { h1.AESEncrypt(lvl12string.Item1.GetSHA512Hash()),
                                                                         h2.Protect(),
                                                                         h3.Protect(),
                                                                         h4.AESEncrypt(lvl34string.Item2.GetSHA512Hash()) }).ConfigureAwait(false);

        dynamicDataCache.SetData("pass", new ProtectedString(passwordBase, this));
        dynamicDataCache.SetData("mnemonic", null);

        MainThreadExecutor.QueueAction(() => onWalletEncrypted?.Invoke(encryptedHashLvls, saltedPasswordHash, encryptedSeed));
    }
}