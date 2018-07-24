using Hope.Utils.EthereumUtils;
using TMPro;

public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
    public sealed class AddressManager
    {
        private readonly TMP_InputField addressField;

        private const int VALID_ADDRESS_LENGTH = 42;

        public bool IsValid { get; private set; }

        public AddressManager(TMP_InputField addressField)
        {
            this.addressField = addressField;

            addressField.onValueChanged.AddListener(CheckAddress);
        }

        private void CheckAddress(string address)
        {
            addressField.text = address.LimitEnd(VALID_ADDRESS_LENGTH);

            IsValid = AddressUtils.IsValidEthereumAddress(addressField.text);
        }
    }
}