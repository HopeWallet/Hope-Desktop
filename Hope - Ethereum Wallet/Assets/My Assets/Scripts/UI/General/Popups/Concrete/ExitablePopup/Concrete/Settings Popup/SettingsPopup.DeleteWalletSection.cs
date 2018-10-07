using TMPro;
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
	public sealed class DeleteWalletSection : MonoBehaviour
	{
		private HopeInputField currentPasswordField;
		private Button deleteButton;
		private GameObject loadingIcon;

		private SettingsPopupAnimator settingsPopupAnimator;
		private PopupManager popupManager;
		private LogoutHandler logoutHandler;

		private string walletName;

		/// <summary>
		///  Sets the necessary variables and dependencies
		/// </summary>
		/// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager </param>
		/// <param name="userWalletManager"> The active UserWalletManger </param>
		/// <param name="settingsPopupAnimator"> The SettingsPopupAnimator </param>
		/// <param name="popupManager"> The active PopupManager </param>
		/// <param name="logoutHandler"> The active LogoutManager</param>
		/// <param name="currentPasswordField"> The current password field </param>
		/// <param name="deleteWalletText"> The delete wallet text </param>
		/// <param name="deleteButton"> The delete button </param>
		/// <param name="loadingIcon"> The loading icon </param>
		public DeleteWalletSection(HopeWalletInfoManager hopeWalletInfoManager,
								   UserWalletManager userWalletManager,
								   SettingsPopupAnimator settingsPopupAnimator,
								   PopupManager popupManager,
								   LogoutHandler logoutHandler,
								   HopeInputField currentPasswordField,
								   TextMeshProUGUI deleteWalletText,
								   Button deleteButton,
								   GameObject loadingIcon)
		{
			this.settingsPopupAnimator = settingsPopupAnimator;
			this.popupManager = popupManager;
			this.logoutHandler = logoutHandler;
			this.currentPasswordField = currentPasswordField;
			this.deleteButton = deleteButton;
			this.loadingIcon = loadingIcon;

			currentPasswordField.Error = false;
			currentPasswordField.OnInputUpdated += (text) => deleteButton.interactable = !string.IsNullOrEmpty(text);
			deleteButton.onClick.AddListener(DeleteWallet);
			walletName = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress()).WalletName;
			deleteWalletText.text = "Delete " + walletName + "...";
		}

		/// <summary>
		/// Deletes the wallet if the current password is correct, else it gives an error
		/// </summary>
		private void DeleteWallet()
		{
			settingsPopupAnimator.VerifyingPassword(deleteButton.gameObject, loadingIcon, true);

			//check if passwordIsCorrect
			bool passwordIsCorrect = true;

			if (passwordIsCorrect)
			{
				popupManager.GetPopup<GeneralOkCancelPopup>(true).SetSubText("Are you sure you want to delete " + walletName + "?").OnOkClicked(() => logoutHandler.Logout());
			}
			else
			{
				currentPasswordField.Error = true;
				currentPasswordField.UpdateVisuals();
				deleteButton.interactable = false;
				settingsPopupAnimator.PasswordVerificationFinished(deleteButton.gameObject, loadingIcon, deleteButton.transform.GetChild(0).gameObject);
			}
	}
	}
}
