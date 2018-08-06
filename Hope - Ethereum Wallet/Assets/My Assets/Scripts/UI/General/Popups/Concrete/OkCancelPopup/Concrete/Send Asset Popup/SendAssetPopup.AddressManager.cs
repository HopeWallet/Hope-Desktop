using Hope.Utils.EthereumUtils;
using TMPro;
using UnityEngine;

/// <summary>
/// Class which displays the popup for sending a TradableAsset.
/// </summary>
public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
	/// <summary>
	/// Class which manages the send address of the <see cref="SendAssetPopup"/>.
	/// </summary>
	public sealed class AddressManager
	{
		public readonly TMP_InputField addressField;
		public readonly TMP_Text contactName;

		/// <summary>
		/// Whether the send address is valid.
		/// </summary>
		public bool IsValid { get; private set; }

		/// <summary>
		/// Whether the address input field is empty or not.
		/// </summary>
		public bool IsEmpty { get { return string.IsNullOrEmpty(addressField.text); } }

		/// <summary>
		/// The address the asset should be sent to.
		/// </summary>
		public string SendAddress { get { return addressField.text; } }

		/// <summary>
		/// Initializes the <see cref="AddressManager"/> by assigning the send address input field.
		/// </summary>
		/// <param name="addressField"> The input field for the address. </param>
		public AddressManager(TMP_InputField addressField, TMP_Text contactName)
		{
			this.addressField = addressField;
			this.contactName = contactName;

			addressField.onValueChanged.AddListener(CheckAddress);
		}

		/// <summary>
		/// Checks if the address is valid once the text is changed.
		/// </summary>
		/// <param name="address"> The string entered in the address field. </param>
		private void CheckAddress(string address)
		{
			IsValid = AddressUtils.IsValidEthereumAddress(addressField.text);

			if (IsValid)
				CheckIfSavedAddress(address);
			else
				contactName.text = "";
		}

		/// <summary>
		/// Checks if the inputted address is from a saved contact.
		/// </summary>
		/// <param name="address"> The address in the input field. </param>
		private void CheckIfSavedAddress(string address)
		{
			if (SecurePlayerPrefs.HasKey(address))
				contactName.text = "[ " + SecurePlayerPrefs.GetString(address) + " ]";
			

			else
				contactName.text = "";

		}
	}
}