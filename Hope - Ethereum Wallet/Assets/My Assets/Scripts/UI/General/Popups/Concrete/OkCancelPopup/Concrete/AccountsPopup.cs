using Hope.Utils.Ethereum;
using Nethereum.HdWallet;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class AccountsPopup : OkCancelPopupComponent<AccountsPopup>
{
    public static Action<int> OnAccountChanged;

    public event Action<string[], int, int> OnPageChanged;

    [SerializeField] private GeneralRadioButtons addressesCategories;
    [SerializeField] private Transform addressesSection;
    [SerializeField] private Button previousPageButton, nextPageButton;
    [SerializeField] private TextMeshProUGUI pageNumText;

    private UserWalletManager userWalletManager;
    private EthereumTransactionManager ethereumTransactionManager;
    private EthereumTransactionButtonManager ethereumTransactionButtonManager;
    private TradableAssetManager tradableAssetManager;
    private TradableAssetButtonManager tradableAssetButtonManager;
    private TradableAssetNotificationManager tradableAssetNotificationManager;
    private LockedPRPSManager lockedPRPSManager;
    private LockPRPSManager lockPRPSManager;

    private readonly Dictionary<string, decimal> addressBalances = new Dictionary<string, decimal>();

    private readonly string[][] addresses = new string[2][];

    private string unlockedAccount;

    private int pageNum,
                firstAddressNumInList,
                currentlySelectedAddress,
                addressesIndex;

    [Inject]
    public void Construct(
        UserWalletManager userWalletManager,
        EthereumTransactionManager ethereumTransactionManager,
        EthereumTransactionButtonManager ethereumTransactionButtonManager,
        TradableAssetManager tradableAssetManager,
        TradableAssetButtonManager tradableAssetButtonManager,
        TradableAssetNotificationManager tradableAssetNotificationManager,
        LockedPRPSManager lockedPRPSManager,
        LockPRPSManager lockPRPSManager)
    {
        this.userWalletManager = userWalletManager;
        this.ethereumTransactionManager = ethereumTransactionManager;
        this.ethereumTransactionButtonManager = ethereumTransactionButtonManager;
        this.tradableAssetManager = tradableAssetManager;
        this.tradableAssetButtonManager = tradableAssetButtonManager;
        this.tradableAssetNotificationManager = tradableAssetNotificationManager;
        this.lockedPRPSManager = lockedPRPSManager;
        this.lockPRPSManager = lockPRPSManager;
    }

    protected override void Awake()
    {
        base.Awake();

        InitializeValues();
        AssignListeners();
        AssignAddresses();
        InitializePageAndCategories();
    }

    private void InitializePageAndCategories()
    {
        addressesCategories.ButtonClicked(addressesIndex);
        pageNumText.text = pageNum.ToString();
    }

    private void InitializeValues()
    {
        pageNum = (userWalletManager.AccountNumber / 5) + 1;
        firstAddressNumInList = pageNum == 0 ? 1 : (pageNum * 5) - 4;
        currentlySelectedAddress = userWalletManager.AccountNumber + 1;
        unlockedAccount = userWalletManager.GetWalletAddress();
    }

    private void AssignListeners()
    {
        for (int i = 0; i < 5; i++)
            AddButtonListeners(i);

        addressesCategories.OnButtonChanged += AddressCategoryChanged;
        previousPageButton.onClick.AddListener(() => PageChanged(false));
        nextPageButton.onClick.AddListener(() => PageChanged(true));
    }

    private void AddButtonListeners(int i)
    {
        addressesSection.GetChild(i).GetComponent<Button>().onClick.AddListener(() => AddressClicked(i));
    }

    private void AssignAddresses()
    {
        addresses[0] = new string[50];
        addresses[1] = new string[50];

        for (int i = 0; i < 50; i++)
            addresses[0][i] = userWalletManager.GetWalletAddress(i, Wallet.DEFAULT_PATH);
        for (int i = 0; i < 50; i++)
            addresses[1][i] = userWalletManager.GetWalletAddress(i, Wallet.ELECTRUM_LEDGER_PATH);

        addressesIndex = userWalletManager.WalletPath.EqualsIgnoreCase(Wallet.DEFAULT_PATH) ? 0 : 1;
    }

    protected override void OnOkClicked()
    {
        userWalletManager.SetWalletAccount(currentlySelectedAddress - 1);
        userWalletManager.SetWalletPath(addressesIndex == 0 ? Wallet.DEFAULT_PATH : Wallet.ELECTRUM_LEDGER_PATH);

        tradableAssetNotificationManager.LoadNewNotificationList();

        lockedPRPSManager.ClearList();
        lockedPRPSManager.PeriodicUpdate();
        lockPRPSManager.PeriodicUpdate();

        ethereumTransactionManager.ClearTransactionList();
        ethereumTransactionManager.PeriodicUpdate();
        ethereumTransactionButtonManager.ProcessNewAssetList();

        tradableAssetButtonManager.ResetButtonNotifications();
        tradableAssetManager.PeriodicUpdate();

        OnAccountChanged?.Invoke(currentlySelectedAddress - 1);
    }

    private void AddressClicked(int num)
    {
        SetAddressButtonInteractability(addressesSection.GetChild((currentlySelectedAddress - 1) % 5), true);

        Transform selectedAddressTransform = addressesSection.GetChild(num);

        SetAddressButtonInteractability(selectedAddressTransform, false);
        currentlySelectedAddress = int.Parse(selectedAddressTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text);

        SetUnlockButtonInteractability();
    }

    private void PageChanged(bool movingForward)
    {
        if (pageNum == 1 && !movingForward)
            pageNum = 10;
        else if (pageNum == 10 && movingForward)
            pageNum = 1;
        else
            pageNum = movingForward ? ++pageNum : --pageNum;

        firstAddressNumInList = (pageNum * 5) - 4;
        pageNumText.text = pageNum.ToString();

        LoadNewAddresses();
    }

    private void AddressCategoryChanged(int num)
    {
        addressesIndex = num;

        SetUnlockButtonInteractability();
        LoadNewAddresses();
    }

    private void LoadNewAddresses()
    {
        string[] addressGroup = addresses[addressesIndex].Skip(firstAddressNumInList - 1).Take(5).ToArray();

        LoadAddressBalances(addressGroup);
        OnPageChanged?.Invoke(addressGroup, firstAddressNumInList, currentlySelectedAddress);
    }

    private void LoadAddressBalances(string[] addressGroup)
    {
        for (int i = 0; i < addressGroup.Length; i++)
        {
            var address = addressGroup[i];
            var num = i;
            if (addressBalances.ContainsKey(address))
            {
                SetAddressBalance(i, address);
            }
            else
            {
                tradableAssetManager.ActiveTradableAsset.GetBalance(address, balance =>
                {
                    if (!addressBalances.ContainsKey(address))
                        addressBalances.Add(address, balance);
                    else
                        addressBalances[address] = balance;

                    if (addresses[addressesIndex][firstAddressNumInList - 1 + num].Equals(address))
                        SetAddressBalance(num, address);
                });
            }
        }
    }

    private void SetAddressBalance(int addressIndex, string address)
    {
        if (addressesSection == null)
            return;

        Transform child = addressesSection.GetChild(addressIndex);

        if (child == null)
            return;

        TMP_Text textComponent = child.GetChild(2).GetComponent<TMP_Text>();

        string balanceText = addressBalances[address].ConvertDecimalToString().LimitEnd(5, "..");
        string symbolText = " <size=60%>" + tradableAssetManager.ActiveTradableAsset.AssetSymbol.LimitEnd(5) + "</size>";

        textComponent.text = string.Concat(balanceText, symbolText);
    }

    private void SetAddressButtonInteractability(Transform addressTransform, bool interactable)
    {
        addressTransform.GetComponent<Button>().interactable = interactable;

        for (int i = 0; i < addressTransform.childCount; i++)
            addressTransform.GetChild(i).gameObject.AnimateColor(interactable ? UIColors.LightGrey : UIColors.White, 0.15f);
    }

    private void SetUnlockButtonInteractability()
    {
        okButton.interactable = !unlockedAccount.EqualsIgnoreCase(userWalletManager.GetWalletAddress(currentlySelectedAddress - 1, addressesIndex == 0 ? Wallet.DEFAULT_PATH : Wallet.ELECTRUM_LEDGER_PATH));
    }
}