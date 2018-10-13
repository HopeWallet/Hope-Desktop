using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The popup that manages the modification of user's settings and preferences
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	/// <summary>
	/// The section where the user can change their wallet name
	/// </summary>
	public sealed class WalletNameSection
	{
        public static event Action OnWalletNameChanged;

		private readonly GameObject currentPasswordSection, walletNameSection, loadingIcon;
		private readonly HopeInputField currentPasswordField, currentWalletNameField, newWalletNameField;
		private readonly Button nextButton, saveButton;
		private readonly GameObject[] hopeOnlyCategoryButtons;

		private readonly HopeWalletInfoManager hopeWalletInfoManager;
		private readonly HopeWalletInfoManager.Settings walletSettings;
		private readonly WalletPasswordVerification walletPasswordVerification;
        private readonly ContactsManager contactsManager;
		private readonly DynamicDataCache dynamicDataCache;
		private readonly SettingsPopupAnimator settingsPopupAnimator;

        private readonly WalletInfo walletInfo;

		public WalletNameSection(
			HopeWalletInfoManager hopeWalletInfoManager,
			HopeWalletInfoManager.Settings walletSettings,
			WalletPasswordVerification walletPasswordVerification,
            ContactsManager contactsManager,
			DynamicDataCache dynamicDataCache,
			UserWalletManager userWalletManager,
			SettingsPopupAnimator settingsPopupAnimator,
			GameObject currentPasswordSection,
			GameObject walletNameSection,
			GameObject loadingIcon,
			HopeInputField currentPasswordField,
			HopeInputField currentWalletNameField,
			HopeInputField newWalletNameField,
			Button nextButton,
			Button saveButton,
			GameObject[] hopeOnlyCategoryButtons)
		{
			this.hopeWalletInfoManager = hopeWalletInfoManager;
			this.walletSettings = walletSettings;
			this.walletPasswordVerification = walletPasswordVerification;
            this.contactsManager = contactsManager;
			this.dynamicDataCache = dynamicDataCache;
			this.settingsPopupAnimator = settingsPopupAnimator;
			this.currentPasswordSection = currentPasswordSection;
			this.walletNameSection = walletNameSection;
			this.loadingIcon = loadingIcon;
			this.currentPasswordField = currentPasswordField;
			this.currentWalletNameField = currentWalletNameField;
			this.newWalletNameField = newWalletNameField;
			this.nextButton = nextButton;
			this.saveButton = saveButton;
			this.hopeOnlyCategoryButtons = hopeOnlyCategoryButtons;

            walletInfo = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress());
			SetListeners();
		}

		/// <summary>
		/// Sets the necessary input field listeners
		/// </summary>
		private void SetListeners()
		{
			currentPasswordField.OnInputUpdated += CurrentPasswordFieldChanged;
			nextButton.onClick.AddListener(EditWalletButtonClicked);

			currentWalletNameField.Error = false;
			currentWalletNameField.Text = walletInfo.WalletName;
			newWalletNameField.OnInputUpdated += WalletNameFieldChanged;
			saveButton.onClick.AddListener(SaveWalletNameClicked);
		}

		/// <summary>
		/// The current password input field has been changed
		/// </summary>
		/// <param name="text"> The text in the input field </param>
		private void CurrentPasswordFieldChanged(string text)
		{
			currentPasswordField.Error = string.IsNullOrEmpty(text);
			nextButton.interactable = !currentPasswordField.Error;
		}

		/// <summary>
		/// The edit wallet button has been clicked
		/// </summary>
		private void EditWalletButtonClicked()
		{
			currentPasswordField.InputFieldBase.interactable = false;
			settingsPopupAnimator.ShowLoadingIcon(nextButton.gameObject, loadingIcon, true);

			walletPasswordVerification.VerifyPassword(currentPasswordField).OnPasswordCorrect(_ =>
									{
                                        if (nextButton == null)
                                            return;

										settingsPopupAnimator.ShowLoadingIcon(nextButton.gameObject, loadingIcon, false);
										currentPasswordSection.AnimateScale(0f, 0.15f, () => walletNameSection.AnimateScale(1f, 0.15f));
										hopeOnlyCategoryButtons[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Wallet Name";
										hopeOnlyCategoryButtons[3].AnimateScaleX(1f, 0.15f);
										hopeOnlyCategoryButtons[4].AnimateScaleX(1f, 0.15f);
									})
									.OnPasswordIncorrect(() =>
									{
                                        if (nextButton == null)
                                            return;

										nextButton.interactable = false;
										settingsPopupAnimator.ShowLoadingIcon(nextButton.gameObject, loadingIcon, false);
									});
		}

		/// <summary>
		/// Wallet name field has been changed
		/// </summary>
		/// <param name="text"> The current text in the input field</param>
		private void WalletNameFieldChanged(string text)
		{
			bool emptyName = string.IsNullOrEmpty(text.Trim());
			bool usedName = WalletNameExists(text);
			newWalletNameField.Error = emptyName || usedName;

			if (emptyName)
				newWalletNameField.errorMessage.text = "Invalid wallet name";
			else if (usedName)
				newWalletNameField.errorMessage.text = "Wallet name in use";

			saveButton.interactable = !newWalletNameField.Error && !currentPasswordField.Error;
		}

		/// <summary>
		/// Loops through the saved wallets and checks if a wallet is already saved under the given name.
		/// </summary>
		/// <param name="text"> The current text in the input field </param>
		/// <returns> Whether the given walletName has been used before </returns>
		private bool WalletNameExists(string text)
		{
			for (int i = 1; ; i++)
			{
				if (hopeWalletInfoManager.GetWalletInfo(i).WalletName.EqualsIgnoreCase(text))
					return true;
				else if (string.IsNullOrEmpty(hopeWalletInfoManager.GetWalletInfo(i).WalletName))
					return false;
			}
		}

		/// <summary>
		/// Save wallet name button has been clicked
		/// </summary>
		private void SaveWalletNameClicked()
		{
            var contactList = contactsManager.ContactList;
            var sameNameContacts = contactList.Where(contact => contact.ContactName.Equals(walletInfo.WalletName)).ToList();

            sameNameContacts.ForEach(contact => contactList[contactList.IndexOf(contact)] = new ContactInfo(contact.ContactAddress, newWalletNameField.Text));

            hopeWalletInfoManager.UpdateWalletInfo(
                walletInfo.WalletNum,
                new WalletInfo(walletInfo.EncryptedWalletData, newWalletNameField.Text, walletInfo.WalletAddresses, walletInfo.WalletNum));

			currentWalletNameField.Text = newWalletNameField.Text;
			newWalletNameField.Text = string.Empty;
			settingsPopupAnimator.AnimateIcon(saveButton.transform.GetChild(0).gameObject);

            OnWalletNameChanged?.Invoke();
		}
	}
}
