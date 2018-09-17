using Nethereum.HdWallet;
using Nethereum.Signer;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AccountsPopup : OkCancelPopupComponent<AccountsPopup>
{
    public event Action<int, int> OnPageChanged;

	[SerializeField] private GeneralRadioButtons addressesCategories;
	[SerializeField] private Transform addressesSection;
	[SerializeField] private Button previousPageButton, nextPageButton;
	[SerializeField] private TextMeshProUGUI pageNumText;
	[SerializeField] private GameObject loadingIcon;

    private UserWalletManager userWalletManager;

	private int pageNum,
                firstAddressNumInList,
                currentlySelectedAddress,
                currentlyUnlockedAddress;

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
        currentlyUnlockedAddress = userWalletManager.AccountNumber + 1;

        addressesCategories.ButtonClicked(userWalletManager.WalletPath.EqualsIgnoreCase(Wallet.DEFAULT_PATH) ? 0 : 1);
        addressesCategories.OnButtonChanged += AddressCategoryChanged;
		previousPageButton.onClick.AddListener(() => PageChanged(false));
		nextPageButton.onClick.AddListener(() => PageChanged(true));

		for (int i = 0; i < 5; i++)
		{
			SetButtonListener(i);
			addressesSection.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = EthECKey.GenerateKey().GetPublicAddress();
		}
	}

    private void SetButtonListener(int num)
    {
        addressesSection.GetChild(num).GetComponent<Button>().onClick.AddListener(() => AddressClicked(num));
    }

    private void AddressClicked(int num)
	{
		SetAddressInteractable(addressesSection.GetChild(currentlySelectedAddress % 5), true);

		Transform selectedAddressTransform = addressesSection.GetChild(num);

		SetAddressInteractable(selectedAddressTransform, false);
		currentlySelectedAddress = int.Parse(selectedAddressTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text);

		okButton.interactable = currentlyUnlockedAddress != currentlySelectedAddress;
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

        OnPageChanged?.Invoke(firstAddressNumInList, currentlySelectedAddress);
	}

	private void AddressCategoryChanged(int num)
	{
		if (num == 0)
		{
            //Default addresses
            userWalletManager.SwitchWalletPath(Wallet.DEFAULT_PATH);
        }
        else
		{
            //Ledger legacy addresses
            userWalletManager.SwitchWalletPath(Wallet.ELECTRUM_LEDGER_PATH);
		}
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