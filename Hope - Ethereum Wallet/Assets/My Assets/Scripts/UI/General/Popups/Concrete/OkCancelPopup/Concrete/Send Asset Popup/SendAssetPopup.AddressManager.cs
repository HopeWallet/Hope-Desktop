using Hope.Utils.EthereumUtils;
using TMPro;

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
        private readonly TMP_InputField addressField;

        private const int VALID_ADDRESS_LENGTH = 42;

        /// <summary>
        /// Whether the send address is valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// The address the asset should be sent to.
        /// </summary>
        public string SendAddress { get { return addressField.text; } }

        /// <summary>
        /// Initializes the <see cref="AddressManager"/> by assigning the send address input field.
        /// </summary>
        /// <param name="addressField"> The input field for the address. </param>
        public AddressManager(TMP_InputField addressField)
        {
            this.addressField = addressField;

            addressField.onValueChanged.AddListener(CheckAddress);
        }

        /// <summary>
        /// Checks if the address is valid once the text is changed.
        /// </summary>
        /// <param name="address"> The string entered in the address field. </param>
        private void CheckAddress(string address)
        {
            addressField.text = address.LimitEnd(VALID_ADDRESS_LENGTH);

            IsValid = AddressUtils.IsValidEthereumAddress(addressField.text);
        }
    }
}