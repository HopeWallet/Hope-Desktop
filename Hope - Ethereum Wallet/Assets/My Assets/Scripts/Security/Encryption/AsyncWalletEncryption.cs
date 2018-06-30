﻿using Hope.Security.Encryption.DPAPI;
using System;
using System.Threading.Tasks;

namespace Hope.Security.Encryption
{

    /// <summary>
    /// Class which executes wallet encryption tasks asynchronously.
    /// </summary>
    public static class AsyncWalletEncryption
    {

        /// <summary>
        /// Gets the encryption password asynchronously.
        /// </summary>
        /// <param name="safePassword"> The SafePassword object to extract the encryption password from. </param>
        /// <param name="userPassword"> The user's password for accessing the wallet, for extra layer of security. </param>
        /// <param name="generateNew"> Whether to generate a new encryption password or extract an old one. </param>
        public static async void GetEncryptionPasswordAsync(PlayerPrefPassword safePassword, string userPassword,
            Action<string> onPasswordExtracted, bool generateNew = false)
        {
            if (string.IsNullOrEmpty(userPassword))
                onPasswordExtracted?.Invoke(null);
            else
                onPasswordExtracted?.Invoke(await Task.Run(()
                    => generateNew ? safePassword.GenerateEncryptionPassword(userPassword) : safePassword.ExtractEncryptionPassword(userPassword)));
        }

        /// <summary>
        /// Encrypts the wallet's private key and seed using a password.
        /// </summary>
        /// <param name="privateKey"> The private key of the wallet. </param>
        /// <param name="phrase"> The mnemonic seed of the wallet. </param>
        /// <param name="encryptionPassword"> The password to encrypt the wallet with. </param>
        /// <param name="onWalletEncrypted"> Action called once the wallet has been encrypted. </param>
        public static async void EncryptWalletAsync(string privateKey, string phrase, string encryptionPassword, Action<WalletData> onWalletEncrypted)
        {
            if (string.IsNullOrEmpty(encryptionPassword) || encryptionPassword.Length < AESEncryption.MIN_PASSWORD_LENGTH)
            {
                ExceptionManager.DisplayException(new Exception("Unable to encrypt wallet. Invalid encryption password!"));
                return;
            }

            var splitPvtKey = privateKey.SplitHalf();
            var splitPhrase = phrase.SplitHalf();

            var pvtKey1Task = await Task.Run(() => splitPvtKey.firstHalf.AESEncrypt(encryptionPassword));
            var pvtKey2Task = await Task.Run(() => splitPvtKey.secondHalf.DPEncrypt(encryptionPassword));
            var phrase1Task = await Task.Run(() => splitPhrase.firstHalf.AESEncrypt(encryptionPassword));
            var phrase2Task = await Task.Run(() => splitPhrase.secondHalf.DPEncrypt(encryptionPassword));

            onWalletEncrypted?.Invoke(new WalletData(pvtKey1Task, pvtKey2Task, phrase1Task, phrase2Task));
        }

        /// <summary>
        /// Decrypts the data from a WalletData object asynchronously.
        /// </summary>
        /// <param name="walletData"> The wallet to decrypt. </param>
        /// <param name="encryptionPassword"> The password used to encrypt the wallet. </param>
        /// <param name="onWalletDecrypted"> Action called once the wallet has been decrypted with the private key and seed as parameters. </param>
        public static async void DecryptWalletAsync(WalletData walletData, string encryptionPassword, Action<string, string> onWalletDecrypted)
        {
            if (string.IsNullOrEmpty(encryptionPassword) || encryptionPassword.Length < AESEncryption.MIN_PASSWORD_LENGTH)
            {
                ExceptionManager.DisplayException(new Exception("Unable to decrypt wallet. Invalid encryption password!"));
                return;
            }

            var pvtKey1Task = await Task.Run(() => walletData.PrivateKey1.AESDecrypt(encryptionPassword));
            var pvtKey2Task = await Task.Run(() => walletData.PrivateKey2.DPDecrypt(encryptionPassword));
            var phrase1Task = await Task.Run(() => walletData.Phrase1.AESDecrypt(encryptionPassword));
            var phrase2Task = await Task.Run(() => walletData.Phrase2.DPDecrypt(encryptionPassword));

            onWalletDecrypted?.Invoke(pvtKey1Task + pvtKey2Task, phrase1Task + phrase2Task);
        }

    }

}