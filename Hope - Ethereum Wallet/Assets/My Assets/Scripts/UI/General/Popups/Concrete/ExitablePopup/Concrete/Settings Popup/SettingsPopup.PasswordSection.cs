using Hope.Security.ProtectedTypes.Types;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The popup that manages the modification of user's settings and preferences
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
    /// <summary>
    /// The section where the user can change their password
    /// </summary>
    public sealed class PasswordSection
    {
        private readonly HopeInputField newPasswordField, confirmPasswordField;
        private readonly Button saveButton;
        private readonly GameObject loadingIcon;

        private readonly PlayerPrefPasswordDerivation playerPrefPasswordDerivation;
        private readonly WalletEncryptor walletEncryptor;
        private readonly WalletDecryptor walletDecryptor;
        private readonly HopeWalletInfoManager hopeWalletInfoManager;
        private readonly DynamicDataCache dynamicDataCache;

        private readonly WalletInfo walletInfo;

        private readonly SettingsPopupAnimator settingsPopupAnimator;

        public PasswordSection(
            PlayerPrefPasswordDerivation playerPrefPasswordDerivation,
            UserWalletManager userWalletManager,
            HopeWalletInfoManager hopeWalletInfoManager,
            DynamicDataCache dynamicDataCache,
            SettingsPopupAnimator settingsPopupAnimator,
            HopeInputField newPasswordField,
            HopeInputField confirmPasswordField,
            Button saveButton,
            GameObject loadingIcon)
        {
            this.playerPrefPasswordDerivation = playerPrefPasswordDerivation;
            this.hopeWalletInfoManager = hopeWalletInfoManager;
            this.dynamicDataCache = dynamicDataCache;
            this.settingsPopupAnimator = settingsPopupAnimator;
            this.newPasswordField = newPasswordField;
            this.confirmPasswordField = confirmPasswordField;
            this.saveButton = saveButton;
            this.loadingIcon = loadingIcon;

            walletEncryptor = new WalletEncryptor(playerPrefPasswordDerivation, dynamicDataCache);
            walletDecryptor = new WalletDecryptor(playerPrefPasswordDerivation, dynamicDataCache);
            walletInfo = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress());

            newPasswordField.OnInputUpdated += _ => PasswordsUpdated();
            confirmPasswordField.OnInputUpdated += _ => PasswordsUpdated();
            saveButton.onClick.AddListener(SavePasswordButtonClicked);
        }

        /// <summary>
        /// Checks if the passwords valid and animates the error icon if needed
        /// </summary>
        private void PasswordsUpdated()
        {
            newPasswordField.Error = newPasswordField.InputFieldBase.text.Length < 8;
            confirmPasswordField.Error = !confirmPasswordField.Equals(newPasswordField);

            if (newPasswordField.Error)
                newPasswordField.errorMessage.text = "Password too short";

            if (confirmPasswordField.Error)
                confirmPasswordField.errorMessage.text = "Passwords do not match";

            confirmPasswordField.UpdateVisuals();
            saveButton.interactable = !newPasswordField.Error && !confirmPasswordField.Error;
        }

        /// <summary>
        /// Save password button has been clicked
        /// </summary>
        [SecureCallEnd]
        private void SavePasswordButtonClicked()
        {
            settingsPopupAnimator.ShowLoadingIcon(saveButton.gameObject, loadingIcon, true);

            var promise = (dynamicDataCache.GetData("pass") as ProtectedString)?.CreateDisposableData();

            promise.OnSuccess(disposableData =>
                walletDecryptor.DecryptWallet(walletInfo, (byte[])disposableData.ByteValue.Clone(), seed =>
                    walletEncryptor.EncryptWallet(seed, confirmPasswordField.InputFieldBytes, OnNewWalletEncrypted)));
        }

        private void OnNewWalletEncrypted(string[] hashes, string passwordHash, string encryptedSeed)
        {
            hopeWalletInfoManager.UpdateWalletInfo(
                walletInfo.WalletNum,
                new WalletInfo(
                    new WalletInfo.EncryptedDataContainer(hashes, encryptedSeed, passwordHash),
                    walletInfo.WalletName,
                    walletInfo.WalletAddresses,
                    walletInfo.WalletNum));

            playerPrefPasswordDerivation.SetupPlayerPrefs(walletInfo.WalletAddresses[0][0], () =>
            {
                MainThreadExecutor.QueueAction(() =>
                {
                    newPasswordField.Text = string.Empty;
                    confirmPasswordField.Text = string.Empty;
                    settingsPopupAnimator.ShowLoadingIcon(saveButton.gameObject, loadingIcon, false);
                    settingsPopupAnimator.AnimateIcon(saveButton.transform.GetChild(0).gameObject);
                });
            }, false);
        }
    }
}
