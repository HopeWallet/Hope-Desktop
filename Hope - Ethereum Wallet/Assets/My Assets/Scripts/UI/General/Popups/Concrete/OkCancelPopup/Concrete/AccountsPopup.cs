using Nethereum.HdWallet;
using Nethereum.Signer;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AccountsPopup : OkCancelPopupComponent<AccountsPopup>
{
    public event Action<string[], int, int> OnPageChanged;

	[SerializeField] private GeneralRadioButtons addressesCategories;
	[SerializeField] private Transform addressesSection;
	[SerializeField] private Button previousPageButton, nextPageButton;
	[SerializeField] private TextMeshProUGUI pageNumText;
	[SerializeField] private GameObject loadingIcon;

    private UserWalletManager userWalletManager;

    private readonly string[][] addresses = new string[2][];

    private string unlockedAccount;

	private int pageNum,
                firstAddressNumInList,
                currentlySelectedAddress,
                addressesIndex;

    [Inject]
    public void Construct(UserWalletManager userWalletManager)
    {
        this.userWalletManager = userWalletManager;
    }

    protected override void Awake()
    {
        base.Awake();

        pageNum = 1;
        firstAddressNumInList = 1;
        currentlySelectedAddress = 1;
        unlockedAccount = userWalletManager.WalletAddress;

        AssignListeners();
        AssignAddresses();

        for (int i = 0; i < 5; i++)
            addressesSection.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = addresses[addressesIndex][i];
    }

    private void AssignListeners()
    {
        for (int i = 0; i < 5; i++)
            AddButtonListeners(i);

        addressesCategories.ButtonClicked(userWalletManager.WalletPath.EqualsIgnoreCase(Wallet.DEFAULT_PATH) ? 0 : 1);
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
            addresses[0][i] = userWalletManager.GetAddress(i, Wallet.DEFAULT_PATH);
        for (int i = 0; i < 50; i++)
            addresses[1][i] = userWalletManager.GetAddress(i, Wallet.ELECTRUM_LEDGER_PATH);

        addressesIndex = userWalletManager.WalletPath.EqualsIgnoreCase(Wallet.DEFAULT_PATH) ? 0 : 1;
    }

    protected override void OnOkClicked()
    {

    }

    private void AddressClicked(int num)
    {
        SetAddressInteractable(addressesSection.GetChild(currentlySelectedAddress % 5), true);

        Transform selectedAddressTransform = addressesSection.GetChild(num);

        SetAddressInteractable(selectedAddressTransform, false);
        currentlySelectedAddress = int.Parse(selectedAddressTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text);

        UpdateButtonInteractability();
    }

    private void UpdateButtonInteractability()
    {
        okButton.interactable = !unlockedAccount.EqualsIgnoreCase(userWalletManager.GetAddress(currentlySelectedAddress - 1, addressesIndex == 0 ? Wallet.DEFAULT_PATH : Wallet.ELECTRUM_LEDGER_PATH));
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

        OnPageChanged?.Invoke(addresses[addressesIndex].Skip(firstAddressNumInList - 1).Take(5).ToArray(), firstAddressNumInList, currentlySelectedAddress);
	}

	private void AddressCategoryChanged(int num)
	{
        addressesIndex = num;
        UpdateButtonInteractability();
        OnPageChanged?.Invoke(addresses[addressesIndex].Skip(firstAddressNumInList - 1).Take(5).ToArray(), firstAddressNumInList, currentlySelectedAddress);
    }

	private void SetAddressInteractable(Transform addressTransform, bool interactable)
	{
		addressTransform.GetComponent<Button>().interactable = interactable;

		for (int i = 0; i < addressTransform.childCount; i++)
			addressTransform.GetChild(i).gameObject.AnimateColor(interactable ? UIColors.LightGrey : UIColors.White, 0.15f);
	}

	private void SetLoadingIcon(bool active)
	{
		if (active)
			loadingIcon.SetActive(true);

		loadingIcon.AnimateGraphicAndScale(active ? 1f : 0f, active ? 1f : 0f, 0.1f, () => { if (!active) loadingIcon.SetActive(false); });
	}
}