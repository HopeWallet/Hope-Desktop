using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The popup that manages the modification of user's settings and preferences
/// </summary>
public sealed partial class SettingsPopup : ExitablePopupComponent<SettingsPopup>
{
	/// <summary>
	/// The delete wallet section of the SettingsPopup
	/// </summary>
	public sealed class DeleteWalletSection
	{
		private HopeInputField currentPasswordField;
		private Button deleteButton;
		private GameObject loadingIcon;
		private PopupManager popupManager;
		private LogoutHandler logoutHandler;

		private string walletName;

		/// <summary>
		///  Sets the necessary variables and dependencies
		/// </summary>
		/// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager </param>
		/// <param name="userWalletManager"> The active UserWalletManger </param>
		/// <param name="logoutHandler"> The active LogoutManager</param>
		/// <param name="currentPasswordField"> The current password field </param>
		/// <param name="deleteButton"> The delete button </param>
		/// <param name="loadingIcon"> The loading icon </param>
		public DeleteWalletSection(HopeWalletInfoManager hopeWalletInfoManager,
								   UserWalletManager userWalletManager,
								   LogoutHandler logoutHandler,
								   HopeInputField currentPasswordField,
								   Button deleteButton,
								   GameObject loadingIcon)
		{
			this.logoutHandler = logoutHandler;
			this.currentPasswordField = currentPasswordField;
			this.deleteButton = deleteButton;
			this.loadingIcon = loadingIcon;

			currentPasswordField.OnInputUpdated += (text) => deleteButton.interactable = !string.IsNullOrEmpty(text);
			walletName = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName;
		}

		/// <summary>
		/// Delete button has been clicked and opens up a confirmation popup
		/// </summary>
		private void DeleteButtonClicked()
		{
			deleteButton.interactable = false;
			popupManager.GetPopup<GeneralOkCancelPopup>().SetSubText("Are you sure you want to delete " + walletName + "?").OnOkClicked(() => logoutHandler.Logout()).OnFinish(() => deleteButton.interactable = true);
		}
	}
}
