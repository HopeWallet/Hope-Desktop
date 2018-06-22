using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
using System.Linq;
using System.Numerics;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which displays the popup for sending a TradableAsset.
/// </summary>
public class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>, IGasPriceObservable, IEnterButtonObservable, ITabButtonObservable, IEtherBalanceObservable
{

    // FIX BUG:
    // - Sometimes the tab button to switch input fields doesn't work and an error is thrown
    // - Missing an input field apparently
    // - Sometimes it takes a long time to get the gas estimates as well

    public InputField addressField,
                      amountField,
                      gasLimitField,
                      gasPriceField;

    public Toggle advancedModeToggle;

    public Image assetImage;

    public Text assetSymbol,
                assetBalance;

    public Dropdown speedDropdown;

    private GasPriceObserver gasPriceObserver;
    private ButtonClickObserver buttonObserver;
    private EtherBalanceObserver etherBalanceObserver;
    private TradableAssetManager tradableAssetManager;
    private TradableAssetImageManager tradableAssetImageManager;
    private UserWalletManager userWalletManager;

    private InputField[] simpleModeFields,
                         advancedModeFields;

    private TradableAsset activeAsset;

    private BigInteger lastEstimatedLimit;

    private HexBigInteger gasLimit,
                          gasPrice;

    private decimal transferAmount;

    private bool isValidAmount,
                 isValidAddress,
                 updating,
                 updatedBefore;

    private const int MAX_AMOUNT_FIELD_LENGTH = 30;
    private const int MAX_ADDRESS_FIELD_LENGTH = 42;
    private const int MAX_BALANCE_TEXT_LENGTH = 20;

    /// <summary>
    /// The update interval for how often the gas estimates should be updated.
    /// </summary>
    public float UpdateInterval => 20f;

    /// <summary>
    /// The gas price for standard transaction speed.
    /// </summary>
    public GasPrice StandardGasPrice { get; set; }

    /// <summary>
    /// The gas price for slow transaction speed.
    /// </summary>
    public GasPrice SlowGasPrice { get; set; }

    /// <summary>
    /// The gas price for fast transaction speed.
    /// </summary>
    public GasPrice FastGasPrice { get; set; }

    /// <summary>
    /// The ether balance of the current wallet.
    /// </summary>
    public dynamic EtherBalance { get; set; }

    /// <summary>
    /// Adds the required dependencies to this class.
    /// </summary>
    /// <param name="gasPriceObserver"> The GasPriceObserver to use to receive gas prices. </param>
    /// <param name="buttonObserver"> The active ButtonClickObserver. </param>
    /// <param name="etherBalanceObserver"> The active EtherBalanceObserver. </param>
    /// <param name="tradableAssetManager"> The TradableAssetManager to use to retrieve the active asset. </param>
    /// <param name="tradableAssetImageManager"> The TradableAssetImageManager to use to retrieve the asset image. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    [Inject]
    public void Construct(GasPriceObserver gasPriceObserver,
        ButtonClickObserver buttonObserver,
        EtherBalanceObserver etherBalanceObserver,
        TradableAssetManager tradableAssetManager,
        TradableAssetImageManager tradableAssetImageManager,
        UserWalletManager userWalletManager)
    {
        this.gasPriceObserver = gasPriceObserver;
        this.buttonObserver = buttonObserver;
        this.etherBalanceObserver = etherBalanceObserver;
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.userWalletManager = userWalletManager;
    }

    /// <summary>
    /// Initializes this OkCancelPopup by adding all the methods to the listener events.
    /// </summary>
    protected override void OnStart()
    {
        simpleModeFields = new InputField[] { addressField, amountField };
        advancedModeFields = new InputField[] { addressField, amountField, gasLimitField, gasPriceField };

        amountField.onValueChanged.AddListener(str => ValidateFields());
        addressField.onValueChanged.AddListener(str => ValidateFields());
        gasLimitField.onValueChanged.AddListener(str => ValidateFields());
        gasPriceField.onValueChanged.AddListener(str => ValidateFields());
        speedDropdown.onValueChanged.AddListener(val => ValidateFields());
        advancedModeToggle.onValueChanged.AddListener(val => ValidateFields());

        ValidateFields();
        SetSendAssetInfo();
        OnGasPricesUpdated();
        SubscribeAll();
    }

    /// <summary>
    /// Sets the image, symbol, and current balance of the asset that will be sent out.
    /// </summary>
    private void SetSendAssetInfo()
    {
        assetSymbol.text = activeAsset.AssetSymbol;
        assetBalance.text = StringUtils.LimitEnd(activeAsset.AssetBalance + "", MAX_BALANCE_TEXT_LENGTH, "...");
        tradableAssetImageManager.LoadImage(activeAsset.AssetSymbol, img => assetImage.sprite = img);
    }

    /// <summary>
    /// Unsubscribes this component from the observers once the popoup is closed.
    /// </summary>
    protected override void OnCancelClicked() => UnsubscribeAll();

    /// <summary>
    /// Called when the Send button is clicked, which transfers or attempts to transfer the current asset given the input from this popup.
    /// Also unsubscribes this class from the observers.
    /// </summary>
    protected override void OnOkClicked()
    {
        UnsubscribeAll();
        userWalletManager.TransferAsset(activeAsset, gasLimit, gasPrice, addressField.text, transferAmount);
    }

    /// <summary>
    /// Subscribes this class to the different observers.
    /// </summary>
    private void SubscribeAll()
    {
        ObserverHelpers.SubscribeObservables(this, etherBalanceObserver, gasPriceObserver, buttonObserver);

        //etherBalanceObserver.SubscribeObservable(this);
        //gasPriceObserver.SubscribeObservable(this);
        //buttonObserver.SubscribeObservable(this);
    }

    /// <summary>
    /// Unsubsribes this class from the different observers.
    /// </summary>
    private void UnsubscribeAll()
    {
        ObserverHelpers.UnsubscribeObservables(this, etherBalanceObserver, gasPriceObserver, buttonObserver);
        //etherBalanceObserver.UnsubscribeObservable(this);
        //gasPriceObserver.UnsubscribeObservable(this);
        //buttonObserver.UnsubscribeObservable(this);
    }

    /// <summary>
    /// Checks all fields and updates the interactivity of each gui element.
    /// </summary>
    private void ValidateFields()
    {
        activeAsset = tradableAssetManager.ActiveTradableAsset;

        FixAmountField();
        FixGasFields();

        isValidAddress = CheckForValidAddress();
        isValidAmount = CheckForValidAmount();

        UpdateInteractableElements();
        CheckForValidGas();
    }

    /// <summary>
    /// Updates the interactivity of all the gui elements based on whether the address and amount fields are valid.
    /// </summary>
    private void UpdateInteractableElements()
    {
        var isValidAddressAndAmount = isValidAddress && isValidAmount;

        amountField.interactable = isValidAddress;
        gasLimitField.interactable = isValidAddressAndAmount;
        gasPriceField.interactable = isValidAddressAndAmount;
        speedDropdown.interactable = isValidAddressAndAmount;
        okButton.interactable = isValidAddressAndAmount;
    }

    /// <summary>
    /// Fixes the speed dropdown, gas limit and price fields based on whether the advanced mode toggle is enabled or not.
    /// </summary>
    private void FixGasFields()
    {
        speedDropdown.gameObject.SetActive(!advancedModeToggle.isOn);
        gasLimitField.gameObject.SetActive(advancedModeToggle.isOn);
        gasPriceField.gameObject.SetActive(advancedModeToggle.isOn);
    }

    /// <summary>
    /// Fixes the amount field so that the length of the amount does not go over the overflow limit of the decimal value, or the amount of decimals the asset can hodl.
    /// </summary>
    private void FixAmountField()
    {
        var sendAmount = amountField.text;

        if (sendAmount == null)
            return;

        sendAmount = new string(sendAmount.Where(c => char.IsDigit(c) || c == '.').ToArray());
        amountField.text = sendAmount;

        var decimals = activeAsset.AssetDecimals;
        var decimalIndex = sendAmount.IndexOf(".");
        var assetDecimalLength = decimals + decimalIndex + 1;

        if (decimals == 0)
            if (decimalIndex != -1)
                sendAmount = sendAmount.Substring(0, sendAmount.Length - 1);

        var substringLength = assetDecimalLength > MAX_AMOUNT_FIELD_LENGTH || decimalIndex == -1 ? MAX_AMOUNT_FIELD_LENGTH : assetDecimalLength;
        if (sendAmount.Length > substringLength)
            sendAmount = sendAmount.Substring(0, substringLength);

        amountField.text = sendAmount;
    }

    /// <summary>
    /// Makes sure the address field does not go over the length of an address, and checks if it is a valid address.
    /// </summary>
    /// <returns> True if the address field holds a valid address. </returns>
    private bool CheckForValidAddress()
    {
        addressField.text = addressField.text.LimitEnd(MAX_ADDRESS_FIELD_LENGTH);
        return AddressUtils.IsValidEthereumAddress(addressField.text);
    }

    /// <summary>
    /// Checks for a valid amount in the amount field.
    /// </summary>
    /// <returns> True if the amount entered in the field is valid and is less than the current balance. </returns>
    private bool CheckForValidAmount()
    {
        var sendAmount = amountField.text;

        if (sendAmount == null)
            return false;

        decimal.TryParse(sendAmount, out transferAmount);
        decimal value = (decimal)activeAsset.AssetBalance;

        return transferAmount > 0 && transferAmount <= value;
    }

    /// <summary>
    /// Checks if the gas entered is valid and the user can pay for the transaction with the current balance.
    /// </summary>
    private void CheckForValidGas()
    {
        if (!isValidAddress || !isValidAmount)
            return;

        if (advancedModeToggle.isOn)
            AdvancedModeGasCheck();
        else
            SimpleModeGasCheck();
    }

    /// <summary>
    /// Gets the gas price and limit entered in the advanced mode fields and checks if the user can pay for that gas.
    /// </summary>
    private void AdvancedModeGasCheck()
    {
        var fixedGasLimitText = new string(gasLimitField.text.Where(c => char.IsDigit(c)).ToArray());
        var fixedGasPriceText = new string(gasPriceField.text.Where(c => char.IsDigit(c)).ToArray());
        gasLimitField.text = fixedGasLimitText;
        gasPriceField.text = fixedGasPriceText;

        decimal price, limit;
        decimal.TryParse(fixedGasLimitText, out limit);
        decimal.TryParse(fixedGasPriceText, out price);

        SetGasValues(new BigInteger(limit), GasUtils.GetFunctionalGasPrice((double)price));
    }

    /// <summary>
    /// Grabs the latest gas price estimates and checks if the user can pay for it.
    /// </summary>
    private void SimpleModeGasCheck()
    {
        if (updating)
            return;

        var correctPrice = (GasUtils.GasPriceTarget)speedDropdown.value == GasUtils.GasPriceTarget.Slow
                                ? SlowGasPrice.FunctionalGasPrice.Value :
                           (GasUtils.GasPriceTarget)speedDropdown.value == GasUtils.GasPriceTarget.Standard
                                ? StandardGasPrice.FunctionalGasPrice.Value : FastGasPrice.FunctionalGasPrice.Value;

        SetGasValues(lastEstimatedLimit, correctPrice);
    }

    /// <summary>
    /// Sets the gas values and send button interactivity based on whether the transaction can afford to be sent.
    /// </summary>
    /// <param name="limit"> The gas limit to use for the transaction. </param>
    /// <param name="price"> The gas price to use for the transaction. </param>
    private void SetGasValues(BigInteger limit, BigInteger price)
    {
        var etherAsset = tradableAssetManager.EtherAsset;
        var gasCost = GasUtils.CalculateMaximumGasCost(price, limit);
        var gasValid = etherAsset == activeAsset ? etherAsset.AssetBalance >= gasCost + transferAmount : etherAsset.AssetBalance > gasCost;

        gasPrice = new HexBigInteger(price);
        gasLimit = new HexBigInteger(limit);

        okButton.interactable = isValidAddress && isValidAmount && gasValid && gasPrice.Value > 0 && gasLimit.Value >= lastEstimatedLimit;
    }

    /// <summary>
    /// Updates the text for the gas field based on the new limit and price.
    /// </summary>
    /// <param name="limit"> The new gas limit. </param>
    private void UpdateGasFieldText(BigInteger limit)
    {
        gasLimitField.text = limit.ToString();
        gasPriceField.text = StandardGasPrice.ReadableGasPrice.ToString();
    }

    /// <summary>
    /// Switches the active input field once enter is pressed.
    /// </summary>
    /// <param name="clickType"> The enter button click type. </param>
    public void EnterButtonPressed(ClickType clickType) => SwitchSelectedInputField(clickType);

    /// <summary>
    /// Switches the active input field once tab is pressed.
    /// </summary>
    /// <param name="clickType"> The tab button click type. </param>
    public void TabButtonPressed(ClickType clickType) => SwitchSelectedInputField(clickType);

    /// <summary>
    /// Switches the input field if the click type is valid.
    /// </summary>
    /// <param name="clickType"> The button click type. </param>
    private void SwitchSelectedInputField(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        (advancedModeToggle.isOn ? advancedModeFields : simpleModeFields).MoveToNextInputField();
    }

    /// <summary>
    /// Updates the gas of the transaction.
    /// </summary>
    public void OnGasPricesUpdated()
    {
        updating = true;

        activeAsset.GetTransferGasLimit(isValidAddress ? addressField.text : userWalletManager.WalletAddress,
                                        isValidAmount ? transferAmount : (decimal)activeAsset.AssetBalance,
                                        limit =>
        {
            tradableAssetManager.EtherAsset.UpdateBalance(() =>
            {
                BigInteger currentLimit;
                BigInteger.TryParse(gasLimitField.text, out currentLimit);

                lastEstimatedLimit = limit;

                if (!advancedModeToggle.isOn || !updatedBefore)
                {
                    updatedBefore = true;

                    SetGasValues(limit, StandardGasPrice.FunctionalGasPrice.Value);
                    UpdateGasFieldText(limit);
                }
                else if (currentLimit < lastEstimatedLimit)
                {
                    gasLimitField.text = limit.ToString();
                }

                assetBalance.text = StringUtils.LimitEnd(activeAsset.AssetBalance + "", MAX_BALANCE_TEXT_LENGTH, "...");

                updating = false;
            });
        });
    }
}
